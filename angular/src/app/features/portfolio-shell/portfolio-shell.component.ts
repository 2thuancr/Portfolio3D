import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { Meta, Title } from '@angular/platform-browser';
import { PublicPortfolioApiService } from '../../core/api/public-portfolio-api.service';
import { PublicPortfolioDto } from '../../core/api/models/public-portfolio.models';
import { HeroSectionComponent } from './sections/hero-section/hero-section.component';
import { AboutSectionComponent } from './sections/about-section/about-section.component';
import { FeaturedProjectsSectionComponent } from './sections/featured-projects-section/featured-projects-section.component';
import { SkillsSectionComponent } from './sections/skills-section/skills-section.component';
import { ContactCtaSectionComponent } from './sections/contact-cta-section/contact-cta-section.component';

type PortfolioLoadState = 'loading' | 'success' | 'error';

@Component({
  selector: 'app-portfolio-shell',
  standalone: true,
  imports: [
    HeroSectionComponent,
    AboutSectionComponent,
    FeaturedProjectsSectionComponent,
    SkillsSectionComponent,
    ContactCtaSectionComponent,
  ],
  templateUrl: './portfolio-shell.component.html',
  styleUrl: './portfolio-shell.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PortfolioShellComponent implements OnInit {
  private readonly api = inject(PublicPortfolioApiService);
  private readonly title = inject(Title);
  private readonly meta = inject(Meta);

  protected readonly state = signal<PortfolioLoadState>('loading');
  protected readonly portfolio = signal<PublicPortfolioDto | null>(null);

  ngOnInit(): void {
    this.title.setTitle('Portfolio 3D');
    this.meta.updateTag({
      name: 'description',
      content: 'A 3D-inspired software engineering portfolio showcasing projects and skills.',
    });

    this.load();
  }

  protected load(): void {
    this.state.set('loading');

    this.api.get().subscribe({
      next: data => {
        this.portfolio.set(data);
        this.state.set('success');
      },
      error: () => {
        this.state.set('error');
      },
    });
  }
}
