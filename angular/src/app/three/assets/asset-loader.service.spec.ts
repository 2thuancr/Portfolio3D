import { TestBed } from '@angular/core/testing';
import * as THREE from 'three';
import { GLTF, GLTFLoader } from 'three/addons/loaders/GLTFLoader.js';
import { AssetLoadError, AssetLoaderService } from './asset-loader.service';

describe('AssetLoaderService', () => {
  let service: AssetLoaderService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [AssetLoaderService] });
    service = TestBed.inject(AssetLoaderService);
  });

  it('should resolve with the loaded model group', async () => {
    const fakeGroup = new THREE.Group();
    spyOn(GLTFLoader.prototype, 'load').and.callFake(
      (_url: string, onLoad: (data: GLTF) => void) => {
        onLoad({ scene: fakeGroup } as GLTF);
      },
    );

    const result = await service.loadModel('/fake.glb');

    expect(result).toBe(fakeGroup);
  });

  it('should report load progress as a 0-100 percentage', async () => {
    spyOn(GLTFLoader.prototype, 'load').and.callFake(
      (
        _url: string,
        onLoad: (data: GLTF) => void,
        onProgress?: (event: ProgressEvent) => void,
      ) => {
        onProgress?.({ lengthComputable: true, loaded: 50, total: 100 } as ProgressEvent);
        onLoad({ scene: new THREE.Group() } as GLTF);
      },
    );

    const progressValues: number[] = [];
    await service.loadModel('/fake.glb', p => progressValues.push(p));

    expect(progressValues).toEqual([50]);
  });

  it('should reject with an AssetLoadError when the loader reports an error', async () => {
    spyOn(GLTFLoader.prototype, 'load').and.callFake(
      (
        _url: string,
        _onLoad: (data: GLTF) => void,
        _onProgress?: (event: ProgressEvent) => void,
        onError?: (err: unknown) => void,
      ) => {
        onError?.(new Error('network down'));
      },
    );

    let caught: unknown;
    try {
      await service.loadModel('/fake.glb');
    } catch (error) {
      caught = error;
    }

    expect(caught).toBeInstanceOf(AssetLoadError);
    expect((caught as AssetLoadError).kind).toBe('network');
  });

  it('should reject with a timeout AssetLoadError if the loader never calls back', async () => {
    spyOn(GLTFLoader.prototype, 'load');

    let caught: unknown;
    try {
      await service.loadModel('/fake.glb', undefined, 5);
    } catch (error) {
      caught = error;
    }

    expect(caught).toBeInstanceOf(AssetLoadError);
    expect((caught as AssetLoadError).kind).toBe('timeout');
  });
});
