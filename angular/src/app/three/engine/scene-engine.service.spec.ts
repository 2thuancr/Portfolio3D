import { TestBed } from '@angular/core/testing';
import * as THREE from 'three';
import { SceneEngineService } from './scene-engine.service';

describe('SceneEngineService', () => {
  let service: SceneEngineService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [SceneEngineService] });
    service = TestBed.inject(SceneEngineService);
  });

  afterEach(() => {
    service.dispose();
  });

  it('should throw when accessing scene or camera before init', () => {
    expect(() => service.scene).toThrowError();
    expect(() => service.camera).toThrowError();
    expect(service.isInitialized).toBeFalse();
  });

  it('should create a scene and a perspective camera on init', () => {
    service.init(16 / 9);

    expect(service.isInitialized).toBeTrue();
    expect(service.scene instanceof THREE.Scene).toBeTrue();
    expect(service.camera instanceof THREE.PerspectiveCamera).toBeTrue();
    expect(service.camera.aspect).toBeCloseTo(16 / 9);
  });

  it('should include a floor and a desk placeholder mesh in the scene', () => {
    service.init(1);

    const meshes = service.scene.children.filter(
      (child): child is THREE.Mesh => child instanceof THREE.Mesh,
    );

    expect(meshes.length).toBe(2);
  });

  it('should update the camera aspect ratio', () => {
    service.init(1);

    service.updateAspect(2);

    expect(service.camera.aspect).toBe(2);
  });

  it('should dispose all mesh geometries/materials and reset state', () => {
    service.init(1);

    const meshes = service.scene.children.filter(
      (child): child is THREE.Mesh => child instanceof THREE.Mesh,
    );
    const disposeSpies = meshes.map(mesh => spyOn(mesh.geometry, 'dispose').and.callThrough());

    service.dispose();

    disposeSpies.forEach(spy => expect(spy).toHaveBeenCalled());
    expect(service.isInitialized).toBeFalse();
  });
});
