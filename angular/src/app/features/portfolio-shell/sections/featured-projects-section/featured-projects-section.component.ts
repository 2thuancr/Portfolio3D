import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { ProjectPublicListDto } from '../../../../core/api/models/public-portfolio.models';

@Component({
  selector: 'app-featured-projects-section',
  standalone: true,
  templateUrl: './featured-projects-section.component.html',
  styleUrl: './featured-projects-section.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeaturedProjectsSectionComponent {
  readonly projects = input<ProjectPublicListDto[]>([]);
}
