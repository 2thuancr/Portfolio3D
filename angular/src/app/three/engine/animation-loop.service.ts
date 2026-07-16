import { Injectable, OnDestroy } from '@angular/core';

export type AnimationLoopCallback = (deltaSeconds: number) => void;

/**
 * Owns a single requestAnimationFrame loop with explicit start/stop and
 * automatic pause while the document is hidden. Framework-agnostic - knows
 * nothing about Three.js, scenes or rendering.
 */
@Injectable()
export class AnimationLoopService implements OnDestroy {
  private frameId: number | null = null;
  private lastTimestamp: number | null = null;
  private callback: AnimationLoopCallback | null = null;
  private running = false;

  private readonly handleVisibilityChange = (): void => {
    if (!this.running) {
      return;
    }

    if (document.hidden) {
      this.cancelScheduledFrame();
    } else if (this.frameId === null) {
      this.lastTimestamp = null;
      this.scheduleNextFrame();
    }
  };

  constructor() {
    document.addEventListener('visibilitychange', this.handleVisibilityChange);
  }

  get isRunning(): boolean {
    return this.running;
  }

  start(callback: AnimationLoopCallback): void {
    if (this.running) {
      // Never start a second loop for an already-running instance.
      return;
    }

    this.callback = callback;
    this.running = true;
    this.lastTimestamp = null;

    if (!document.hidden) {
      this.scheduleNextFrame();
    }
  }

  stop(): void {
    this.running = false;
    this.cancelScheduledFrame();
    this.lastTimestamp = null;
    this.callback = null;
  }

  private scheduleNextFrame(): void {
    this.frameId = requestAnimationFrame(this.tick);
  }

  private cancelScheduledFrame(): void {
    if (this.frameId !== null) {
      cancelAnimationFrame(this.frameId);
      this.frameId = null;
    }
  }

  private readonly tick = (timestamp: number): void => {
    if (!this.running) {
      return;
    }

    const deltaSeconds = this.lastTimestamp === null ? 0 : (timestamp - this.lastTimestamp) / 1000;
    this.lastTimestamp = timestamp;

    this.callback?.(deltaSeconds);
    this.scheduleNextFrame();
  };

  ngOnDestroy(): void {
    this.stop();
    document.removeEventListener('visibilitychange', this.handleVisibilityChange);
  }
}
