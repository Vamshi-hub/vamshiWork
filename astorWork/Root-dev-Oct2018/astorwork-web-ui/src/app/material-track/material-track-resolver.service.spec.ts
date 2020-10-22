import { TestBed, inject } from '@angular/core/testing';

import { MaterialTrackResolverService } from './material-track-resolver.service';

describe('MaterialTrackResolverService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MaterialTrackResolverService]
    });
  });

  it('should be created', inject([MaterialTrackResolverService], (service: MaterialTrackResolverService) => {
    expect(service).toBeTruthy();
  }));
});
