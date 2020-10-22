import { TestBed, inject } from '@angular/core/testing';

import { AppResolverService } from './app-resolver.service';

describe('AppResolverService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppResolverService]
    });
  });

  it('should be created', inject([AppResolverService], (service: AppResolverService) => {
    expect(service).toBeTruthy();
  }));
});
