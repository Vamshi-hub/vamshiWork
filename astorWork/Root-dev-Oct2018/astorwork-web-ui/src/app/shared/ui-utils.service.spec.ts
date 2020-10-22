import { TestBed, inject } from '@angular/core/testing';

import { UiUtilsService } from './ui-utils.service';

describe('UiUtilsService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UiUtilsService]
    });
  });

  it('should be created', inject([UiUtilsService], (service: UiUtilsService) => {
    expect(service).toBeTruthy();
  }));
});
