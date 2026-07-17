import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { SceneCameraTarget, SceneInteractiveObject, SceneManifest, Vector3Tuple } from './scene-manifest.models';

/**
 * Fetches the scene manifest static asset and applies minimal runtime
 * validation (required fields present, vectors are 3-number tuples).
 * Not a generic schema-validation framework - just enough to fail fast
 * with a clear error instead of crashing later on `undefined`.
 */
@Injectable()
export class SceneManifestService {
  private readonly http = inject(HttpClient);

  async loadManifest(url: string): Promise<SceneManifest> {
    const raw = await firstValueFrom(this.http.get(url));
    return this.validate(raw);
  }

  private validate(raw: unknown): SceneManifest {
    if (!raw || typeof raw !== 'object') {
      throw new Error('Scene manifest is not a valid object.');
    }

    const candidate = raw as Record<string, unknown>;

    if (typeof candidate['version'] !== 'number') {
      throw new Error('Scene manifest is missing a numeric "version".');
    }

    if (typeof candidate['modelUrl'] !== 'string' || candidate['modelUrl'].trim() === '') {
      throw new Error('Scene manifest is missing a valid "modelUrl".');
    }

    return {
      version: candidate['version'],
      modelUrl: candidate['modelUrl'],
      modelScale: this.readVector3(candidate['modelScale']),
      modelPosition: this.readVector3(candidate['modelPosition']),
      modelRotation: this.readVector3(candidate['modelRotation']),
      interactiveObjects: this.readInteractiveObjects(candidate['interactiveObjects']),
      cameraTargets: this.readCameraTargets(candidate['cameraTargets']),
    };
  }

  private readVector3(value: unknown): Vector3Tuple | undefined {
    if (Array.isArray(value) && value.length === 3 && value.every(item => typeof item === 'number')) {
      return value as Vector3Tuple;
    }
    return undefined;
  }

  private readInteractiveObjects(value: unknown): SceneInteractiveObject[] {
    if (!Array.isArray(value)) {
      return [];
    }

    return value.filter(
      (item): item is SceneInteractiveObject =>
        !!item &&
        typeof item === 'object' &&
        typeof (item as Record<string, unknown>)['id'] === 'string' &&
        typeof (item as Record<string, unknown>)['objectName'] === 'string' &&
        typeof (item as Record<string, unknown>)['type'] === 'string',
    );
  }

  private readCameraTargets(value: unknown): Record<string, SceneCameraTarget> {
    if (!value || typeof value !== 'object') {
      return {};
    }

    const result: Record<string, SceneCameraTarget> = {};
    for (const [key, target] of Object.entries(value as Record<string, unknown>)) {
      const position = this.readVector3((target as Record<string, unknown>)?.['position']);
      const lookAt = this.readVector3((target as Record<string, unknown>)?.['lookAt']);
      if (position && lookAt) {
        result[key] = { position, lookAt };
      }
    }
    return result;
  }
}
