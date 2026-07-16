import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { ProjectPublicDetailDto } from './models/public-portfolio.models';

@Injectable({
  providedIn: 'root',
})
export class ProjectPublicApiService {
  private readonly restService = inject(RestService);

  private readonly apiName = 'Default';

  getBySlug(slug: string): Observable<ProjectPublicDetailDto> {
    return this.restService.request<void, ProjectPublicDetailDto>(
      {
        method: 'GET',
        url: '/api/app/project-public/by-slug',
        params: { slug },
      },
      { apiName: this.apiName, skipHandleError: true },
    );
  }
}
