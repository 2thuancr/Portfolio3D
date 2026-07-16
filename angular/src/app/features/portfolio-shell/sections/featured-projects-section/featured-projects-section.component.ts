import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ProjectPublicListDto } from '../../../../core/api/models/public-portfolio.models';

@Component({
  selector: 'app-featured-projects-section',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './featured-projects-section.component.html',
  styleUrl: './featured-projects-section.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FeaturedProjectsSectionComponent {
  readonly projects = input<ProjectPublicListDto[]>([]);
}
