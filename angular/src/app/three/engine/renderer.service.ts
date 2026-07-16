import { Injectable, OnDestroy } from '@angular/core';
import * as THREE from 'three';
import { QualitySettings } from '../quality/quality-manager.service';

/**
 * Owns the WebGLRenderer only. Does not know about camera paths, hotspots
 * or portfolio content - just draws whatever scene/camera it is given.
 */
@Injectable()
export class RendererService implements OnDestroy {
  private renderer: THREE.WebGLRenderer | null = null;

  /** Attempts to create a WebGLRenderer bound to the given canvas.
   * Returns false (instead of throwing) if WebGL isn't available, so the
   * caller can fall back to 2D content. */
  init(canvas: HTMLCanvasElement, settings: QualitySettings): boolean {
    try {
      const renderer = new THREE.WebGLRenderer({ canvas, antialias: true });
      renderer.shadowMap.enabled = settings.shadowsEnabled;
      renderer.setPixelRatio(settings.pixelRatio);
      this.renderer = renderer;
      return true;
    } catch {
      this.renderer = null;
      return false;
    }
  }

  setSize(width: number, height: number): void {
    this.renderer?.setSize(width, height, false);
  }

  render(scene: THREE.Scene, camera: THREE.Camera): void {
    this.renderer?.render(scene, camera);
  }

  dispose(): void {
    this.renderer?.dispose();
    this.renderer = null;
  }

  ngOnDestroy(): void {
    this.dispose();
  }
}
