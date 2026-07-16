import { HttpErrorResponse } from '@angular/common/http';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ActivatedRoute, convertToParamMap } from '@angular/router';
import { of, throwError } from 'rxjs';
import { ProjectDetailComponent } from './project-detail.component';
import { ProjectPublicApiService } from '../../core/api/project-public-api.service';
import { ProjectPublicDetailDto } from '../../core/api/models/public-portfolio.models';

describe('ProjectDetailComponent', () => {
  let fixture: ComponentFixture<ProjectDetailComponent>;
  let apiSpy: jasmine.SpyObj<ProjectPublicApiService>;

  const fullProject: ProjectPublicDetailDto = {
    id: '1',
    name: 'VIEvent',
    slug: 'vievent',
    summary: 'An event platform.',
    description: 'A longer description of VIEvent.',
    thumbnailUrl: 'https://example.com/thumb.png',
    demoUrl: 'https://demo.example.com',
    repositoryUrl: 'https://github.com/example/vievent',
    isFeatured: true,
  };

  function setup(): void {
    fixture = TestBed.createComponent(ProjectDetailComponent);
    fixture.detectChanges();
  }

  function configureTestBed(slug: string | null): void {
    apiSpy = jasmine.createSpyObj<ProjectPublicApiService>('ProjectPublicApiService', ['getBySlug']);

    TestBed.configureTestingModule({
      imports: [ProjectDetailComponent],
      providers: [
        { provide: ProjectPublicApiService, useValue: apiSpy },
        {
          provide: ActivatedRoute,
          useValue: {
            paramMap: of(convertToParamMap({ slug })),
          },
        },
      ],
    });
  }

  it('should load and render name, summary and description', () => {
    configureTestBed('vievent');
    apiSpy.getBySlug.and.returnValue(of(fullProject));

    setup();

    expect(apiSpy.getBySlug).toHaveBeenCalledWith('vievent');
    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('VIEvent');
    expect(text).toContain('An event platform.');
    expect(text).toContain('A longer description of VIEvent.');
  });

  it('should render demo and repository links when URLs are present', () => {
    configureTestBed('vievent');
    apiSpy.getBySlug.and.returnValue(of(fullProject));

    setup();

    const links: HTMLAnchorElement[] = Array.from(fixture.nativeElement.querySelectorAll('a'));
    expect(links.some(a => a.getAttribute('href') === fullProject.demoUrl)).toBeTrue();
    expect(links.some(a => a.getAttribute('href') === fullProject.repositoryUrl)).toBeTrue();
  });

  it('should not render demo or repository links when URLs are absent', () => {
    configureTestBed('vievent');
    apiSpy.getBySlug.and.returnValue(
      of({ ...fullProject, demoUrl: null, repositoryUrl: null }),
    );

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).not.toContain('View Demo');
    expect(text).not.toContain('View Repository');
  });

  it('should show a not-found state when the API returns 404', () => {
    configureTestBed('missing-project');
    apiSpy.getBySlug.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 404 })),
    );

    setup();

    const text = fixture.nativeElement.textContent as string;
    expect(text).toContain('not found');
  });

  it('should show an error state and allow retry on other API failures', () => {
    configureTestBed('vievent');
    apiSpy.getBySlug.and.returnValue(
      throwError(() => new HttpErrorResponse({ status: 500 })),
    );

    setup();

    expect(apiSpy.getBySlug).toHaveBeenCalledTimes(1);

    apiSpy.getBySlug.and.returnValue(of(fullProject));
    const retryButton: HTMLButtonElement = fixture.nativeElement.querySelector('button');
    retryButton.click();
    fixture.detectChanges();

    expect(apiSpy.getBySlug).toHaveBeenCalledTimes(2);
    expect((fixture.nativeElement.textContent as string)).toContain('VIEvent');
  });

  it('should not require authentication to load the project', () => {
    configureTestBed('vievent');
    apiSpy.getBySlug.and.returnValue(of(fullProject));

    setup();

    expect(apiSpy.getBySlug).toHaveBeenCalled();
  });
});
