import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import * as THREE from 'three';
import { SceneCanvasComponent } from './scene-canvas.component';
import { RendererService } from '../engine/renderer.service';
import { SceneEngineService } from '../engine/scene-engine.service';
import { AnimationLoopService } from '../engine/animation-loop.service';
import { AssetLoaderService } from '../assets/asset-loader.service';
import { SceneManifestService } from '../assets/scene-manifest.service';
import { SceneManifest } from '../assets/scene-manifest.models';

interface Deferred<T> {
  promise: Promise<T>;
  resolve: (value: T) => void;
  reject: (reason?: unknown) => void;
}

function createDeferred<T>(): Deferred<T> {
  let resolve!: (value: T) => void;
  let reject!: (reason?: unknown) => void;
  const promise = new Promise<T>((res, rej) => {
    resolve = res;
    reject = rej;
  });
  return { promise, resolve, reject };
}

/** Flushes several microtask ticks, enough for the component's chained
 * manifest -> model awaits to settle after a deferred is resolved/rejected. */
async function flush(): Promise<void> {
  for (let i = 0; i < 5; i++) {
    await Promise.resolve();
  }
}

const fakeManifest: SceneManifest = {
  version: 1,
  modelUrl: '/assets/three/models/developer-room-placeholder.glb',
  modelScale: [1, 1, 1],
  modelPosition: [0, 0, 0],
  modelRotation: [0, 0, 0],
  interactiveObjects: [],
  cameraTargets: { main: { position: [0, 2.5, 6], lookAt: [0, 1, 0] } },
};

function getButtons(fixture: ComponentFixture<SceneCanvasComponent>): HTMLButtonElement[] {
  return Array.from(fixture.nativeElement.querySelectorAll('button'));
}

describe('SceneCanvasComponent', () => {
  let fixture: ComponentFixture<SceneCanvasComponent>;
  let manifestDeferred: Deferred<SceneManifest>;
  let modelDeferred: Deferred<THREE.Group>;
  let loadManifestSpy: jasmine.Spy;
  let loadModelSpy: jasmine.Spy;
  let lastOnProgress: ((percent: number) => void) | undefined;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [SceneCanvasComponent],
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });
    fixture = TestBed.createComponent(SceneCanvasComponent);

    const sceneManifestService = fixture.debugElement.injector.get(SceneManifestService);
    const assetLoader = fixture.debugElement.injector.get(AssetLoaderService);

    manifestDeferred = createDeferred<SceneManifest>();
    modelDeferred = createDeferred<THREE.Group>();
    lastOnProgress = undefined;

    loadManifestSpy = spyOn(sceneManifestService, 'loadManifest').and.returnValue(
      manifestDeferred.promise,
    );
    loadModelSpy = spyOn(assetLoader, 'loadModel').and.callFake(
      (_url: string, onProgress?: (percent: number) => void) => {
        lastOnProgress = onProgress;
        return modelDeferred.promise;
      },
    );
  });

  it('should render a canvas and initialize the scene without throwing', () => {
    expect(() => fixture.detectChanges()).not.toThrow();

    const canvas: HTMLCanvasElement | null = fixture.nativeElement.querySelector('canvas');
    expect(canvas).toBeTruthy();
  });

  it('should show a fallback message when WebGL is unavailable', () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(false);

    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain("isn't available");
  });

  it('should not initialize the scene engine when WebGL is unavailable', () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    const sceneEngine = fixture.debugElement.injector.get(SceneEngineService);
    spyOn(renderer, 'init').and.returnValue(false);
    const sceneInitSpy = spyOn(sceneEngine, 'init');

    fixture.detectChanges();

    expect(sceneInitSpy).not.toHaveBeenCalled();
  });

  it('should stop the animation loop and dispose the renderer/scene on destroy', () => {
    fixture.detectChanges();

    const animationLoop = fixture.debugElement.injector.get(AnimationLoopService);
    const renderer = fixture.debugElement.injector.get(RendererService);
    const sceneEngine = fixture.debugElement.injector.get(SceneEngineService);

    const stopSpy = spyOn(animationLoop, 'stop').and.callThrough();
    const rendererDisposeSpy = spyOn(renderer, 'dispose').and.callThrough();
    const sceneDisposeSpy = spyOn(sceneEngine, 'dispose').and.callThrough();

    fixture.destroy();

    expect(stopSpy).toHaveBeenCalled();
    expect(rendererDisposeSpy).toHaveBeenCalled();
    expect(sceneDisposeSpy).toHaveBeenCalled();
  });

  it('should show a manifest-loading status while the manifest request is pending', () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Loading 3D scene');
    expect(loadManifestSpy).toHaveBeenCalledTimes(1);
  });

  it('should transition to loading-model and report progress once the manifest resolves', async () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    manifestDeferred.resolve(fakeManifest);
    await flush();
    fixture.detectChanges();

    expect(loadModelSpy).toHaveBeenCalledTimes(1);

    lastOnProgress?.(42);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('42%');
  });

  it('should reach the ready state and hide the placeholder once the model loads', async () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);
    const sceneEngine = fixture.debugElement.injector.get(SceneEngineService);
    fixture.detectChanges();
    const addModelSpy = spyOn(sceneEngine, 'addModel').and.callThrough();
    const setPlaceholderVisibleSpy = spyOn(sceneEngine, 'setPlaceholderVisible').and.callThrough();

    manifestDeferred.resolve(fakeManifest);
    await flush();
    modelDeferred.resolve(new THREE.Group());
    await flush();
    fixture.detectChanges();

    expect(addModelSpy).toHaveBeenCalled();
    expect(setPlaceholderVisibleSpy).toHaveBeenCalledWith(false);
    expect(fixture.nativeElement.querySelector('.scene-canvas__overlay')).toBeNull();
  });

  it('should show a fallback message with retry/continue actions when the model fails to load', async () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    manifestDeferred.resolve(fakeManifest);
    await flush();
    modelDeferred.reject(new Error('model fetch failed'));
    await flush();
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain("couldn't be loaded");
    expect(getButtons(fixture).length).toBe(2);
  });

  it('should show an error state when the manifest fails to load', async () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    manifestDeferred.reject(new Error('manifest fetch failed'));
    await flush();
    fixture.detectChanges();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('could not be loaded');
  });

  it('should reuse the cached manifest and only reload the model on retry', async () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    manifestDeferred.resolve(fakeManifest);
    await flush();
    modelDeferred.reject(new Error('model fetch failed'));
    await flush();
    fixture.detectChanges();

    const retryButton = getButtons(fixture)[0];
    retryButton.click();

    expect(loadManifestSpy).toHaveBeenCalledTimes(1);
    expect(loadModelSpy).toHaveBeenCalledTimes(2);
  });

  it('should fully reinitialize the renderer when retrying after a WebGL failure', () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    const initSpy = spyOn(renderer, 'init').and.returnValue(false);

    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain("isn't available");

    initSpy.and.returnValue(true);
    const retryButton = getButtons(fixture)[0];
    retryButton.click();

    expect(initSpy).toHaveBeenCalledTimes(2);
  });

  it('should emit closeRequested when the user clicks "Continue in 2D"', async () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    manifestDeferred.resolve(fakeManifest);
    await flush();
    modelDeferred.reject(new Error('model fetch failed'));
    await flush();
    fixture.detectChanges();

    const closeSpy = jasmine.createSpy('closeRequested');
    fixture.componentInstance.closeRequested.subscribe(closeSpy);

    const continueButton = getButtons(fixture).find(button =>
      button.textContent?.includes('Continue in 2D'),
    );
    continueButton?.click();

    expect(closeSpy).toHaveBeenCalled();
  });

  it('should stop the animation loop and show an error state on WebGL context loss', () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);

    fixture.detectChanges();

    const animationLoop = fixture.debugElement.injector.get(AnimationLoopService);
    const stopSpy = spyOn(animationLoop, 'stop').and.callThrough();

    const canvas = fixture.nativeElement.querySelector('canvas') as HTMLCanvasElement;
    const event = new Event('webglcontextlost', { cancelable: true });
    canvas.dispatchEvent(event);
    fixture.detectChanges();

    expect(event.defaultPrevented).toBeTrue();
    expect(stopSpy).toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('context was lost');
  });

  it('should observe the canvas with ResizeObserver for resize handling', () => {
    const renderer = fixture.debugElement.injector.get(RendererService);
    spyOn(renderer, 'init').and.returnValue(true);
    const observeSpy = spyOn(ResizeObserver.prototype, 'observe');

    fixture.detectChanges();

    expect(observeSpy).toHaveBeenCalled();
  });
});
