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

  it('should throw when adding a model before init', () => {
    const model = new THREE.Group();

    expect(() => service.addModel(model)).toThrowError();
  });

  it('should add a model to the scene, apply its transform, and set hasModel', () => {
    service.init(1);
    const model = new THREE.Group();

    service.addModel(model, { position: [1, 2, 3], rotation: [0, Math.PI / 2, 0], scale: [2, 2, 2] });

    expect(service.hasModel).toBeTrue();
    expect(service.scene.children).toContain(model);
    expect(model.position.toArray()).toEqual([1, 2, 3]);
    expect(model.scale.toArray()).toEqual([2, 2, 2]);
  });

  it('should dispose and remove the previous model when a new one is added', () => {
    service.init(1);
    const firstModel = new THREE.Group();
    service.addModel(firstModel);

    const secondModel = new THREE.Group();
    service.addModel(secondModel);

    expect(service.scene.children).not.toContain(firstModel);
    expect(service.scene.children).toContain(secondModel);
    expect(service.hasModel).toBeTrue();
  });

  it('should remove and dispose the model, resetting hasModel to false', () => {
    service.init(1);
    const model = new THREE.Group();
    service.addModel(model);

    service.removeModel();

    expect(service.hasModel).toBeFalse();
    expect(service.scene.children).not.toContain(model);
  });

  it('should toggle placeholder desk visibility without disposing it', () => {
    service.init(1);
    const desk = service.scene.children.find(
      (child): child is THREE.Mesh => child instanceof THREE.Mesh && child.position.y === 0.5,
    );
    expect(desk).toBeDefined();

    service.setPlaceholderVisible(false);
    expect(desk!.visible).toBeFalse();

    service.setPlaceholderVisible(true);
    expect(desk!.visible).toBeTrue();
  });
});
