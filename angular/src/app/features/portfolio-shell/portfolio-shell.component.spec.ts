import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { PortfolioShellComponent } from './portfolio-shell.component';
import { PublicPortfolioApiService } from '../../core/api/public-portfolio-api.service';
import { PublicPortfolioDto } from '../../core/api/models/public-portfolio.models';

describe('PortfolioShellComponent', () => {
  let fixture: ComponentFixture<PortfolioShellComponent>;
  let apiSpy: jasmine.SpyObj<PublicPortfolioApiService>;

  const fullPortfolio: PublicPortfolioDto = {
    profile: {
      displayName: 'Vi Quoc Thuan',
      headline: 'Software Engineer',
      bio: 'Building things.',
      avatarUrl: null,
      cvUrl: 'https://example.com/cv.pdf',
      email: 'me@example.com',
      socialLinks: ['https://github.com/example'],
    },
    featuredProjects: [
      {
        id: '1',
        name: 'Vievent',
        slug: 'vievent',
        summary: 'An event platform.',
        thumbnailUrl: 'https://example.com/thumb.png',
        isFeatured: true,
        displayOrder: 0,
      },
    ],
    skillGroups: [
      {
        category: 'Backend',
        items: [{ name: '.NET', iconUrl: null, levelLabel: 'Advanced' }],
      },
    ],
  };

  function setup(): void {
    fixture = TestBed.createComponent(PortfolioShellComponent);
    fixture.detectChanges();
  }

  beforeEach(() => {
    apiSpy = jasmine.createSpyObj<PublicPortfolioApiService>('PublicPortfolioApiService', ['get']);

    TestBed.configureTestingModule({
      imports: [PortfolioShellComponent],
      providers: [
        { provide: PublicPortfolioApiService, useValue: apiSpy },
        provideRouter([]),
      ],
    });
  });

  it('should render profile, featured projects and skills on success', () => {
    apiSpy.get.and.returnValue(of(fullPortfolio));

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Vi Quoc Thuan');
    expect(text).toContain('Vievent');
    expect(text).toContain('.NET');
  });

  it('should render fallback copy when profile is null', () => {
    apiSpy.get.and.returnValue(
      of({ ...fullPortfolio, profile: null }),
    );

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Software Engineer');
    expect(text).not.toContain('Vi Quoc Thuan');
  });

  it('should show an empty state when there are no featured projects', () => {
    apiSpy.get.and.returnValue(of({ ...fullPortfolio, featuredProjects: [] }));

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('No featured projects yet');
  });

  it('should show an empty state when there are no skill groups', () => {
    apiSpy.get.and.returnValue(of({ ...fullPortfolio, skillGroups: [] }));

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Skills will be listed here soon');
  });

  it('should show an error state and not a blank page when the API fails', () => {
    apiSpy.get.and.returnValue(throwError(() => new Error('network error')));

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain("couldn't load the portfolio");
    expect(text.trim().length).toBeGreaterThan(0);
  });

  it('should retry the API call when the retry button is clicked', () => {
    apiSpy.get.and.returnValue(throwError(() => new Error('network error')));
    setup();

    expect(apiSpy.get).toHaveBeenCalledTimes(1);

    apiSpy.get.and.returnValue(of(fullPortfolio));
    const retryButton: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    retryButton.click();
    fixture.detectChanges();

    expect(apiSpy.get).toHaveBeenCalledTimes(2);
    expect((fixture.nativeElement.textContent as string)).toContain('Vievent');
  });

  it('should not require authentication to load the portfolio', () => {
    apiSpy.get.and.returnValue(of(fullPortfolio));

    setup();

    expect(apiSpy.get).toHaveBeenCalled();
  });

  it('should toggle the 3D button label without removing 2D content', () => {
    apiSpy.get.and.returnValue(of(fullPortfolio));

    setup();

    const toggleButton: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    expect(toggleButton.textContent).toContain('Enter 3D');

    toggleButton.click();
    fixture.detectChanges();

    expect(toggleButton.textContent).toContain('Exit 3D');
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('Vi Quoc Thuan');
    expect(text).toContain('Vievent');
    expect(text).toContain('.NET');

    toggleButton.click();
    fixture.detectChanges();

    expect(toggleButton.textContent).toContain('Enter 3D');
  });
});
