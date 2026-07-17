import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  OnDestroy,
  Output,
  ViewChild,
  inject,
  signal,
} from '@angular/core';
import { AnimationLoopService } from '../engine/animation-loop.service';
import { RendererService } from '../engine/renderer.service';
import { SceneEngineService } from '../engine/scene-engine.service';
import { QualityManagerService } from '../quality/quality-manager.service';
import { AssetLoaderService } from '../assets/asset-loader.service';
import { SceneManifestService } from '../assets/scene-manifest.service';
import { SceneManifest } from '../assets/scene-manifest.models';

export type SceneLoadState =
  | 'idle'
  | 'loading-manifest'
  | 'loading-model'
  | 'ready'
  | 'fallback'
  | 'error';

const SCENE_MANIFEST_URL = '/assets/three/scene-manifest.json';

/**
 * Thin orchestrator: wires the engine/renderer/animation-loop/quality/
 * asset services together and owns the canvas, resize handling and WebGL
 * context-loss listeners. Contains no Three.js object creation itself -
 * that lives in SceneEngineService/AssetLoaderService.
 */
@Component({
  selector: 'app-scene-canvas',
  standalone: true,
  templateUrl: './scene-canvas.component.html',
  styleUrl: './scene-canvas.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    SceneEngineService,
    RendererService,
    AnimationLoopService,
    QualityManagerService,
    AssetLoaderService,
    SceneManifestService,
  ],
})
export class SceneCanvasComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvas', { static: true })
  private readonly canvasRef!: ElementRef<HTMLCanvasElement>;

  /** Emitted when the user asks to leave the 3D view (from a fallback or
   * error state). The parent decides what "closing" the scene means. */
  @Output() readonly closeRequested = new EventEmitter<void>();

  private readonly sceneEngine = inject(SceneEngineService);
  private readonly renderer = inject(RendererService);
  private readonly animationLoop = inject(AnimationLoopService);
  private readonly qualityManager = inject(QualityManagerService);
  private readonly assetLoader = inject(AssetLoaderService);
  private readonly sceneManifestService = inject(SceneManifestService);

  protected readonly state = signal<SceneLoadState>('idle');
  protected readonly progress = signal(0);
  protected readonly message = signal<string | null>(null);

  private resizeObserver: ResizeObserver | null = null;
  private windowResizeListener: (() => void) | null = null;
  /** True only while the current WebGLRenderer/scene are valid - false
   * before first init and after a context loss, so retry() knows whether
   * it needs a full re-initialization or just a content reload. */
  private rendererReady = false;
  private lastManifest: SceneManifest | null = null;

  private readonly handleContextLost = (event: Event): void => {
    event.preventDefault();
    this.rendererReady = false;
    this.animationLoop.stop();
    this.message.set('The 3D context was lost. You can retry or continue in 2D.');
    this.state.set('error');
  };

  private readonly handleContextRestored = (): void => {
    // No automatic recovery (out of MVP scope). The Retry button already
    // performs a full re-initialization because `rendererReady` is false.
  };

  ngAfterViewInit(): void {
    this.initializeSceneAndLoop();
  }

  ngOnDestroy(): void {
    this.teardownResizeHandling();
    this.teardownContextListeners();
    this.animationLoop.stop();
    this.renderer.dispose();
    this.sceneEngine.dispose();
  }

  protected retry(): void {
    this.message.set(null);

    if (!this.rendererReady) {
      this.initializeSceneAndLoop();
      return;
    }

    void this.loadSceneContent(true);
  }

  protected continueIn2D(): void {
    this.closeRequested.emit();
  }

  private initializeSceneAndLoop(): void {
    const canvas = this.canvasRef.nativeElement;
    const { width, height } = this.getCanvasSize(canvas);

    const level = this.qualityManager.determineInitialLevel();
    const settings = this.qualityManager.getSettings(level);

    const initialized = this.renderer.init(canvas, settings);
    if (!initialized) {
      this.rendererReady = false;
      this.message.set("3D preview isn't available in this browser.");
      this.state.set('error');
      return;
    }

    this.rendererReady = true;
    this.sceneEngine.init(width / height);
    this.renderer.setSize(width, height);

    this.animationLoop.start(deltaSeconds => {
      this.sceneEngine.update(deltaSeconds);
      this.renderer.render(this.sceneEngine.scene, this.sceneEngine.camera);
    });

    this.setupResizeHandling(canvas);
    this.setupContextListeners(canvas);

    void this.loadSceneContent(false);
  }

  private async loadSceneContent(reuseManifest: boolean): Promise<void> {
    this.progress.set(0);

    let manifest = reuseManifest ? this.lastManifest : null;

    if (!manifest) {
      this.state.set('loading-manifest');
      try {
        manifest = await this.sceneManifestService.loadManifest(SCENE_MANIFEST_URL);
        this.lastManifest = manifest;
      } catch {
        this.showFallback('The 3D scene configuration could not be loaded.', 'error');
        return;
      }
    }

    this.state.set('loading-model');

    try {
      const model = await this.assetLoader.loadModel(manifest.modelUrl, percent =>
        this.progress.set(percent),
      );
      this.sceneEngine.addModel(model, {
        position: manifest.modelPosition,
        rotation: manifest.modelRotation,
        scale: manifest.modelScale,
      });
      this.sceneEngine.setPlaceholderVisible(false);
      this.state.set('ready');
    } catch {
      this.showFallback(
        "The 3D model couldn't be loaded. Showing a placeholder scene instead.",
        'fallback',
      );
    }
  }

  private showFallback(text: string, kind: 'fallback' | 'error'): void {
    this.message.set(text);
    this.sceneEngine.setPlaceholderVisible(true);
    this.state.set(kind);
  }

  private setupResizeHandling(canvas: HTMLCanvasElement): void {
    if (typeof ResizeObserver !== 'undefined') {
      this.resizeObserver = new ResizeObserver(() => this.handleResize(canvas));
      this.resizeObserver.observe(canvas);
    } else {
      this.windowResizeListener = () => this.handleResize(canvas);
      window.addEventListener('resize', this.windowResizeListener);
    }
  }

  private teardownResizeHandling(): void {
    this.resizeObserver?.disconnect();
    this.resizeObserver = null;

    if (this.windowResizeListener) {
      window.removeEventListener('resize', this.windowResizeListener);
      this.windowResizeListener = null;
    }
  }

  private setupContextListeners(canvas: HTMLCanvasElement): void {
    canvas.addEventListener('webglcontextlost', this.handleContextLost);
    canvas.addEventListener('webglcontextrestored', this.handleContextRestored);
  }

  private teardownContextListeners(): void {
    const canvas = this.canvasRef?.nativeElement;
    canvas?.removeEventListener('webglcontextlost', this.handleContextLost);
    canvas?.removeEventListener('webglcontextrestored', this.handleContextRestored);
  }

  private handleResize(canvas: HTMLCanvasElement): void {
    const { width, height } = this.getCanvasSize(canvas);
    if (width === 0 || height === 0) {
      return;
    }

    this.renderer.setSize(width, height);
    this.sceneEngine.updateAspect(width / height);
  }

  private getCanvasSize(canvas: HTMLCanvasElement): { width: number; height: number } {
    const rect = canvas.getBoundingClientRect();
    return { width: Math.max(rect.width, 1), height: Math.max(rect.height, 1) };
  }
}
