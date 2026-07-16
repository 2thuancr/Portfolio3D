import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ProfilePublicDto } from '../../../../core/api/models/public-portfolio.models';
import { PORTFOLIO_PROFILE_FALLBACK } from '../../../../core/config/portfolio-fallback.config';

@Component({
  selector: 'app-hero-section',
  standalone: true,
  templateUrl: './hero-section.component.html',
  styleUrl: './hero-section.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class HeroSectionComponent {
  readonly profile = input<ProfilePublicDto | null>(null);

  protected readonly displayName = computed(
    () => this.profile()?.displayName ?? PORTFOLIO_PROFILE_FALLBACK.displayName,
  );
  protected readonly headline = computed(
    () => this.profile()?.headline ?? PORTFOLIO_PROFILE_FALLBACK.headline,
  );
  protected readonly bio = computed(() => this.profile()?.bio ?? PORTFOLIO_PROFILE_FALLBACK.bio);
  protected readonly cvUrl = computed(() => this.profile()?.cvUrl ?? null);
}
