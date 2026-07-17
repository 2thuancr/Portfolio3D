import { Injectable } from '@angular/core';
import * as THREE from 'three';
import { GLTF, GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';

export type AssetLoadErrorKind = 'network' | 'timeout' | 'unknown';

export class AssetLoadError extends Error {
  constructor(
    message: string,
    readonly kind: AssetLoadErrorKind,
  ) {
    super(message);
    this.name = 'AssetLoadError';
  }
}

const DEFAULT_TIMEOUT_MS = 20000;

/**
 * Loads a single GLB model via GLTFLoader. Deliberately narrow: does not
 * touch the scene, camera, or any portfolio content - just resolves a
 * THREE.Group (or rejects with a typed AssetLoadError) and reports 0-100
 * progress along the way.
 */
@Injectable()
export class AssetLoaderService {
  private readonly loader = new GLTFLoader();

  loadModel(
    url: string,
    onProgress?: (progress: number) => void,
    timeoutMs = DEFAULT_TIMEOUT_MS,
  ): Promise<THREE.Group> {
    return new Promise<THREE.Group>((resolve, reject) => {
      let settled = false;

      const timeoutId = setTimeout(() => {
        if (settled) {
          return;
        }
        settled = true;
        reject(new AssetLoadError(`Timed out loading model: ${url}`, 'timeout'));
      }, timeoutMs);

      this.loader.load(
        url,
        (gltf: GLTF) => {
          if (settled) {
            return;
          }
          settled = true;
          clearTimeout(timeoutId);
          resolve(gltf.scene);
        },
        progressEvent => {
          if (settled || !onProgress) {
            return;
          }
          if (progressEvent.lengthComputable && progressEvent.total > 0) {
            const percent = Math.min(100, Math.round((progressEvent.loaded / progressEvent.total) * 100));
            onProgress(percent);
          }
        },
        () => {
          if (settled) {
            return;
          }
          settled = true;
          clearTimeout(timeoutId);
          reject(new AssetLoadError(`Failed to load model: ${url}`, 'network'));
        },
      );
    });
  }
}
