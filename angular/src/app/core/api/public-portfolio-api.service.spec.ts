import { TestBed } from '@angular/core/testing';
import { RestService } from '@abp/ng.core';
import { of } from 'rxjs';
import { PublicPortfolioApiService } from './public-portfolio-api.service';

describe('PublicPortfolioApiService', () => {
  let service: PublicPortfolioApiService;
  let restServiceSpy: jasmine.SpyObj<RestService>;

  beforeEach(() => {
    restServiceSpy = jasmine.createSpyObj<RestService>('RestService', ['request']);
    restServiceSpy.request.and.returnValue(of());

    TestBed.configureTestingModule({
      providers: [{ provide: RestService, useValue: restServiceSpy }],
    });

    service = TestBed.inject(PublicPortfolioApiService);
  });

  it('should call the public-portfolio endpoint with GET', () => {
    service.get().subscribe();

    expect(restServiceSpy.request).toHaveBeenCalledWith(
      { method: 'GET', url: '/api/app/public-portfolio' },
      { apiName: 'Default' },
    );
  });
});
