import { TestBed } from '@angular/core/testing';
import { QualityManagerService } from './quality-manager.service';

describe('QualityManagerService', () => {
  let service: QualityManagerService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [QualityManagerService] });
    service = TestBed.inject(QualityManagerService);
  });

  it('should clamp the high-quality pixel ratio to a maximum of 2', () => {
    spyOnProperty(window, 'devicePixelRatio', 'get').and.returnValue(3);

    const settings = service.getSettings('high');

    expect(settings.pixelRatio).toBe(2);
    expect(settings.shadowsEnabled).toBeTrue();
  });

  it('should use a pixel ratio of 1 and disable shadows in low quality', () => {
    const settings = service.getSettings('low');

    expect(settings.pixelRatio).toBe(1);
    expect(settings.shadowsEnabled).toBeFalse();
  });

  it('should choose low quality when the user prefers reduced motion', () => {
    spyOn(window, 'matchMedia').and.returnValue({ matches: true } as MediaQueryList);

    expect(service.determineInitialLevel()).toBe('low');
  });

  it('should choose high quality when the user does not prefer reduced motion', () => {
    spyOn(window, 'matchMedia').and.returnValue({ matches: false } as MediaQueryList);

    expect(service.determineInitialLevel()).toBe('high');
  });
});
