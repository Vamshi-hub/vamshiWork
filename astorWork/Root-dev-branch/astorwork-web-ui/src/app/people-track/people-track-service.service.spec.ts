import { TestBed, inject } from '@angular/core/testing';

import { PeopleTrackServiceService } from './people-track-service.service';

describe('PeopleTrackServiceService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PeopleTrackServiceService]
    });
  });

  it('should be created', inject([PeopleTrackServiceService], (service: PeopleTrackServiceService) => {
    expect(service).toBeTruthy();
  }));
});
