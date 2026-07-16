import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { FeaturedProjectsSectionComponent } from './featured-projects-section.component';
import { ProjectPublicListDto } from '../../../../core/api/models/public-portfolio.models';

describe('FeaturedProjectsSectionComponent', () => {
  let fixture: ComponentFixture<FeaturedProjectsSectionComponent>;

  const project: ProjectPublicListDto = {
    id: '1',
    name: 'VIEvent',
    slug: 'vievent',
    summary: 'An event platform.',
    thumbnailUrl: 'https://example.com/thumb.png',
    isFeatured: true,
    displayOrder: 0,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [FeaturedProjectsSectionComponent],
      providers: [provideRouter([])],
    });

    fixture = TestBed.createComponent(FeaturedProjectsSectionComponent);
    fixture.componentRef.setInput('projects', [project]);
    fixture.detectChanges();
  });

  it('should link each card to /projects/:slug', () => {
    const link: HTMLAnchorElement = fixture.nativeElement.querySelector('.projects__card-link');

    expect(link.getAttribute('href')).toBe('/projects/vievent');
  });
});
