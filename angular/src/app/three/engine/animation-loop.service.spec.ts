import { TestBed } from '@angular/core/testing';
import { AnimationLoopService } from './animation-loop.service';

describe('AnimationLoopService', () => {
  let service: AnimationLoopService;
  let rafSpy: jasmine.Spy;
  let cafSpy: jasmine.Spy;
  let rafCallbacks: FrameRequestCallback[];

  function flushFrame(timestamp: number): void {
    const callback = rafCallbacks.shift();
    callback?.(timestamp);
  }

  beforeEach(() => {
    rafCallbacks = [];
    let nextId = 1;

    rafSpy = spyOn(window, 'requestAnimationFrame').and.callFake((cb: FrameRequestCallback) => {
      rafCallbacks.push(cb);
      return nextId++;
    });
    cafSpy = spyOn(window, 'cancelAnimationFrame');

    TestBed.configureTestingModule({ providers: [AnimationLoopService] });
    service = TestBed.inject(AnimationLoopService);
  });

  afterEach(() => {
    service.ngOnDestroy();
  });

  it('should start the loop and schedule a frame', () => {
    service.start(() => {});

    expect(service.isRunning).toBeTrue();
    expect(rafSpy).toHaveBeenCalledTimes(1);
  });

  it('should not create a second loop when start is called while already running', () => {
    service.start(() => {});
    service.start(() => {});

    expect(rafSpy).toHaveBeenCalledTimes(1);
  });

  it('should invoke the callback on each frame and reschedule the next one', () => {
    const callback = jasmine.createSpy('callback');
    service.start(callback);

    flushFrame(16);

    expect(callback).toHaveBeenCalledTimes(1);
    expect(rafSpy).toHaveBeenCalledTimes(2);
  });

  it('should stop the loop and cancel the pending frame', () => {
    service.start(() => {});
    service.stop();

    expect(service.isRunning).toBeFalse();
    expect(cafSpy).toHaveBeenCalled();
  });

  it('should not invoke the callback after being stopped', () => {
    const callback = jasmine.createSpy('callback');
    service.start(callback);
    service.stop();

    // A frame requested before stop() should still be a no-op if it fires late.
    flushFrame(16);

    expect(callback).not.toHaveBeenCalled();
  });

  it('should pause the raf loop while the document is hidden and resume when visible', () => {
    const hiddenSpy = spyOnProperty(document, 'hidden', 'get').and.returnValue(false);

    service.start(() => {});
    expect(rafSpy).toHaveBeenCalledTimes(1);

    hiddenSpy.and.returnValue(true);
    document.dispatchEvent(new Event('visibilitychange'));
    expect(cafSpy).toHaveBeenCalledTimes(1);

    hiddenSpy.and.returnValue(false);
    document.dispatchEvent(new Event('visibilitychange'));
    expect(rafSpy).toHaveBeenCalledTimes(2);
  });

  it('should stop the loop and no longer react to visibilitychange after destroy', () => {
    service.start(() => {});
    const callsBeforeDestroy = rafSpy.calls.count();

    service.ngOnDestroy();

    expect(service.isRunning).toBeFalse();

    document.dispatchEvent(new Event('visibilitychange'));

    expect(rafSpy.calls.count()).toBe(callsBeforeDestroy);
  });
});
