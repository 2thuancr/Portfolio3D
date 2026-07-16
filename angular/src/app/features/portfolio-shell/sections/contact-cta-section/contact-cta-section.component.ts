import { ChangeDetectionStrategy, Component, computed, input } from '@angular/core';
import { ProfilePublicDto } from '../../../../core/api/models/public-portfolio.models';

@Component({
  selector: 'app-contact-cta-section',
  standalone: true,
  templateUrl: './contact-cta-section.component.html',
  styleUrl: './contact-cta-section.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ContactCtaSectionComponent {
  readonly profile = input<ProfilePublicDto | null>(null);

  protected readonly email = computed(() => this.profile()?.email ?? null);
}
