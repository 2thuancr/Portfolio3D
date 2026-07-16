import { TestBed } from '@angular/core/testing';
import { RendererService } from './renderer.service';
import { QualitySettings } from '../quality/quality-manager.service';

describe('RendererService', () => {
  let service: RendererService;

  const highQuality: QualitySettings = { level: 'high', pixelRatio: 1, shadowsEnabled: true };

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [RendererService] });
    service = TestBed.inject(RendererService);
  });

  afterEach(() => {
    service.dispose();
  });

  it('should initialize successfully with a real canvas', () => {
    const canvas = document.createElement('canvas');

    const initialized = service.init(canvas, highQuality);

    expect(initialized).toBeTrue();
  });

  it('should return false instead of throwing when WebGL context creation fails', () => {
    const canvas = document.createElement('canvas');
    spyOn(canvas, 'getContext').and.returnValue(null);

    const initialized = service.init(canvas, highQuality);

    expect(initialized).toBeFalse();
  });

  it('should dispose the renderer without throwing', () => {
    const canvas = document.createElement('canvas');
    service.init(canvas, highQuality);

    expect(() => service.dispose()).not.toThrow();
  });

  it('should not throw when rendering or resizing without a successful init', () => {
    expect(() => service.setSize(100, 100)).not.toThrow();
  });
});
