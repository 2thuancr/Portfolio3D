import {
  AfterViewInit,
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  OnDestroy,
  ViewChild,
  inject,
  signal,
} from '@angular/core';
import { AnimationLoopService } from '../engine/animation-loop.service';
import { RendererService } from '../engine/renderer.service';
import { SceneEngineService } from '../engine/scene-engine.service';
import { QualityManagerService } from '../quality/quality-manager.service';

/**
 * Thin orchestrator: wires the engine/renderer/animation-loop/quality
 * services together and owns the canvas + resize observer. Contains no
 * Three.js object creation itself - that lives in SceneEngineService.
 */
@Component({
  selector: 'app-scene-canvas',
  standalone: true,
  templateUrl: './scene-canvas.component.html',
  styleUrl: './scene-canvas.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [SceneEngineService, RendererService, AnimationLoopService, QualityManagerService],
})
export class SceneCanvasComponent implements AfterViewInit, OnDestroy {
  @ViewChild('canvas', { static: true })
  private readonly canvasRef!: ElementRef<HTMLCanvasElement>;

  private readonly sceneEngine = inject(SceneEngineService);
  private readonly renderer = inject(RendererService);
  private readonly animationLoop = inject(AnimationLoopService);
  private readonly qualityManager = inject(QualityManagerService);

  protected readonly webglAvailable = signal(true);

  private resizeObserver: ResizeObserver | null = null;

  ngAfterViewInit(): void {
    const canvas = this.canvasRef.nativeElement;
    const { width, height } = this.getCanvasSize(canvas);

    const level = this.qualityManager.determineInitialLevel();
    const settings = this.qualityManager.getSettings(level);

    const initialized = this.renderer.init(canvas, settings);
    if (!initialized) {
      this.webglAvailable.set(false);
      return;
    }

    this.sceneEngine.init(width / height);
    this.renderer.setSize(width, height);

    this.animationLoop.start(deltaSeconds => {
      this.sceneEngine.update(deltaSeconds);
      this.renderer.render(this.sceneEngine.scene, this.sceneEngine.camera);
    });

    this.resizeObserver = new ResizeObserver(() => this.handleResize(canvas));
    this.resizeObserver.observe(canvas);
  }

  ngOnDestroy(): void {
    this.resizeObserver?.disconnect();
    this.resizeObserver = null;
    this.animationLoop.stop();
    this.renderer.dispose();
    this.sceneEngine.dispose();
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
