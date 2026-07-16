import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SceneCanvasComponent } from './scene-canvas.component';
import { RendererService } from '../engine/renderer.service';
import { SceneEngineService } from '../engine/scene-engine.service';
import { AnimationLoopService } from '../engine/animation-loop.service';

describe('SceneCanvasComponent', () => {
  let fixture: ComponentFixture<SceneCanvasComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [SceneCanvasComponent] });
    fixture = TestBed.createComponent(SceneCanvasComponent);
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
});
