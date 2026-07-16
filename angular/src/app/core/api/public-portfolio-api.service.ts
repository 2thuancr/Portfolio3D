import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { PublicPortfolioDto } from './models/public-portfolio.models';

@Injectable({
  providedIn: 'root',
})
export class PublicPortfolioApiService {
  private readonly restService = inject(RestService);

  private readonly apiName = 'Default';

  get(): Observable<PublicPortfolioDto> {
    return this.restService.request<void, PublicPortfolioDto>(
      {
        method: 'GET',
        url: '/api/app/public-portfolio',
      },
      { apiName: this.apiName },
    );
  }
}
