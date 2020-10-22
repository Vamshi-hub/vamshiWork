import { TestBed, inject } from '@angular/core/testing';

import { MaterialTrackService } from './material-track.service';

describe('MaterialTrackService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MaterialTrackService]
    });
  });

  it('should be created', inject([MaterialTrackService], (service: MaterialTrackService) => {
    expect(service).toBeTruthy();
  }));
});
