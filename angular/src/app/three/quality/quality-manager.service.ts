import { Injectable } from '@angular/core';

export type QualityLevel = 'high' | 'low';

export interface QualitySettings {
  level: QualityLevel;
  pixelRatio: number;
  shadowsEnabled: boolean;
}

const MAX_HIGH_PIXEL_RATIO = 2;
const LOW_PIXEL_RATIO = 1;

/**
 * Decides render quality (pixel ratio, shadows). Does not know about scene
 * layout or content - only device/user-preference signals.
 */
@Injectable()
export class QualityManagerService {
  determineInitialLevel(): QualityLevel {
    const prefersReducedMotion =
      typeof window !== 'undefined' &&
      window.matchMedia?.('(prefers-reduced-motion: reduce)').matches;

    return prefersReducedMotion ? 'low' : 'high';
  }

  getSettings(level: QualityLevel): QualitySettings {
    if (level === 'low') {
      return { level, pixelRatio: LOW_PIXEL_RATIO, shadowsEnabled: false };
    }

    const devicePixelRatio = typeof window === 'undefined' ? 1 : window.devicePixelRatio || 1;

    return {
      level,
      pixelRatio: Math.min(devicePixelRatio, MAX_HIGH_PIXEL_RATIO),
      shadowsEnabled: true,
    };
  }
}
