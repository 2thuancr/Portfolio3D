/**
 * Single source of fallback copy shown when the backend Profile has not
 * been configured yet (PublicPortfolioDto.profile === null). Referenced
 * only from the Hero and About sections so this text never needs to be
 * duplicated across components.
 */
export const PORTFOLIO_PROFILE_FALLBACK = {
  displayName: 'Software Engineer',
  headline: 'Building a 3D portfolio experience.',
  bio: 'Profile content has not been configured yet. Check back soon.',
} as const;
