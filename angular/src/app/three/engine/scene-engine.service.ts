import { Injectable, OnDestroy } from '@angular/core';
import * as THREE from 'three';

interface Disposable {
  geometry: THREE.BufferGeometry;
  material: THREE.Material | THREE.Material[];
}

/**
 * Owns the Three.js Scene, camera and placeholder objects: lifecycle and
 * disposal only. No camera path, no hotspots, no API calls, no render call
 * (that's RendererService's job) - just the scene graph.
 */
@Injectable()
export class SceneEngineService implements OnDestroy {
  private _scene: THREE.Scene | null = null;
  private _camera: THREE.PerspectiveCamera | null = null;
  private readonly disposables: Disposable[] = [];

  get scene(): THREE.Scene {
    if (!this._scene) {
      throw new Error('SceneEngineService.init() must be called before accessing scene.');
    }
    return this._scene;
  }

  get camera(): THREE.PerspectiveCamera {
    if (!this._camera) {
      throw new Error('SceneEngineService.init() must be called before accessing camera.');
    }
    return this._camera;
  }

  get isInitialized(): boolean {
    return this._scene !== null;
  }

  init(aspectRatio: number): void {
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0x111827);

    const camera = new THREE.PerspectiveCamera(50, aspectRatio, 0.1, 100);
    camera.position.set(4, 3, 6);
    camera.lookAt(0, 0.5, 0);

    scene.add(new THREE.HemisphereLight(0xffffff, 0x1f2937, 1.2));

    const directionalLight = new THREE.DirectionalLight(0xffffff, 1);
    directionalLight.position.set(5, 8, 3);
    scene.add(directionalLight);

    scene.add(this.createFloor());
    scene.add(this.createDeskPlaceholder());

    this._scene = scene;
    this._camera = camera;
  }

  updateAspect(aspectRatio: number): void {
    if (!this._camera) {
      return;
    }
    this._camera.aspect = aspectRatio;
    this._camera.updateProjectionMatrix();
  }

  /** Advances the scene by one frame. Intentionally empty in Task 011 -
   * no camera path or object animation yet. */
  update(_deltaSeconds: number): void {
    // Placeholder for future camera-path/hotspot animation tasks.
  }

  private createFloor(): THREE.Mesh {
    const geometry = new THREE.PlaneGeometry(20, 20);
    const material = new THREE.MeshStandardMaterial({ color: 0x1f2937 });
    this.disposables.push({ geometry, material });

    const floor = new THREE.Mesh(geometry, material);
    floor.rotation.x = -Math.PI / 2;
    return floor;
  }

  private createDeskPlaceholder(): THREE.Mesh {
    const geometry = new THREE.BoxGeometry(1.5, 1, 0.8);
    const material = new THREE.MeshStandardMaterial({ color: 0x2563eb });
    this.disposables.push({ geometry, material });

    const desk = new THREE.Mesh(geometry, material);
    desk.position.set(0, 0.5, 0);
    return desk;
  }

  dispose(): void {
    for (const { geometry, material } of this.disposables) {
      geometry.dispose();
      if (Array.isArray(material)) {
        material.forEach(m => m.dispose());
      } else {
        material.dispose();
      }
    }
    this.disposables.length = 0;

    this._scene?.clear();
    this._scene = null;
    this._camera = null;
  }

  ngOnDestroy(): void {
    this.dispose();
  }
}
