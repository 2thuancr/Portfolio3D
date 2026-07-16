import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { SkillGroupPublicDto } from '../../../../core/api/models/public-portfolio.models';

@Component({
  selector: 'app-skills-section',
  standalone: true,
  templateUrl: './skills-section.component.html',
  styleUrl: './skills-section.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SkillsSectionComponent {
  readonly skillGroups = input<SkillGroupPublicDto[]>([]);
}
