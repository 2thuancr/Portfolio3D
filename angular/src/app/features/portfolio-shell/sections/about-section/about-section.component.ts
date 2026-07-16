import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ProfilePublicDto } from '../../../../core/api/models/public-portfolio.models';
import { PORTFOLIO_PROFILE_FALLBACK } from '../../../../core/config/portfolio-fallback.config';

@Component({
  selector: 'app-about-section',
  standalone: true,
  templateUrl: './about-section.component.html',
  styleUrl: './about-section.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AboutSectionComponent {
  readonly profile = input<ProfilePublicDto | null>(null);

  protected readonly bio = computed(() => this.profile()?.bio ?? PORTFOLIO_PROFILE_FALLBACK.bio);
  protected readonly avatarUrl = computed(() => this.profile()?.avatarUrl ?? null);
  protected readonly displayName = computed(
    () => this.profile()?.displayName ?? PORTFOLIO_PROFILE_FALLBACK.displayName,
  );
  protected readonly socialLinks = computed(() => this.profile()?.socialLinks ?? []);
}
