import { TestBed } from '@angular/core/testing';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { SceneManifestService } from './scene-manifest.service';
import { SceneManifest } from './scene-manifest.models';

describe('SceneManifestService', () => {
  let service: SceneManifestService;
  let httpMock: HttpTestingController;
  const url = '/assets/three/scene-manifest.json';

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [SceneManifestService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(SceneManifestService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should load and return a fully valid manifest as-is', async () => {
    const validManifest: SceneManifest = {
      version: 1,
      modelUrl: '/assets/three/models/developer-room-placeholder.glb',
      modelScale: [1, 1, 1],
      modelPosition: [0, 0, 0],
      modelRotation: [0, 0, 0],
      interactiveObjects: [{ id: 'about-desk', objectName: 'Desk', type: 'about' }],
      cameraTargets: {
        main: { position: [0, 2.5, 6], lookAt: [0, 1, 0] },
      },
    };

    const resultPromise = service.loadManifest(url);
    httpMock.expectOne(url).flush(validManifest);
    const result = await resultPromise;

    expect(result).toEqual(validManifest);
  });

  it('should default optional/malformed fields instead of throwing', async () => {
    const partialManifest = {
      version: 1,
      modelUrl: '/assets/three/models/developer-room-placeholder.glb',
      modelScale: [1, 1],
      interactiveObjects: 'not-an-array',
      cameraTargets: { main: { position: [0, 1, 2] } },
    };

    const resultPromise = service.loadManifest(url);
    httpMock.expectOne(url).flush(partialManifest);
    const result = await resultPromise;

    expect(result.modelScale).toBeUndefined();
    expect(result.interactiveObjects).toEqual([]);
    expect(result.cameraTargets).toEqual({});
  });

  it('should reject when the manifest is missing required fields', async () => {
    const invalidManifest = { version: 1 };

    const resultPromise = service.loadManifest(url);
    httpMock.expectOne(url).flush(invalidManifest);

    await expectAsync(resultPromise).toBeRejected();
  });
});
