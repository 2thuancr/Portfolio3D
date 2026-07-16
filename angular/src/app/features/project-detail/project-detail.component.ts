import { HttpErrorResponse } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, DestroyRef, OnInit, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { Meta, Title } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ProjectPublicApiService } from '../../core/api/project-public-api.service';
import { ProjectPublicDetailDto } from '../../core/api/models/public-portfolio.models';

type ProjectDetailState = 'loading' | 'success' | 'not-found' | 'error';

@Component({
  selector: 'app-project-detail',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './project-detail.component.html',
  styleUrl: './project-detail.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ProjectDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly api = inject(ProjectPublicApiService);
  private readonly title = inject(Title);
  private readonly meta = inject(Meta);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly state = signal<ProjectDetailState>('loading');
  protected readonly project = signal<ProjectPublicDetailDto | null>(null);

  private currentSlug: string | null = null;

  ngOnInit(): void {
    this.route.paramMap.pipe(takeUntilDestroyed(this.destroyRef)).subscribe(params => {
      this.currentSlug = params.get('slug');
      this.load();
    });
  }

  protected load(): void {
    const slug = this.currentSlug;

    if (!slug) {
      this.state.set('not-found');
      return;
    }

    this.state.set('loading');

    this.api.getBySlug(slug).subscribe({
      next: data => {
        this.project.set(data);
        this.state.set('success');
        this.title.setTitle(`${data.name} · Portfolio 3D`);
        this.meta.updateTag({ name: 'description', content: data.summary });
      },
      error: (err: HttpErrorResponse) => {
        this.state.set(err.status === 404 ? 'not-found' : 'error');
      },
    });
  }
}
