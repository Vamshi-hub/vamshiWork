import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobscheduleSampleViewComponent } from './jobschedule-sample-view.component';

describe('JobscheduleSampleViewComponent', () => {
  let component: JobscheduleSampleViewComponent;
  let fixture: ComponentFixture<JobscheduleSampleViewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobscheduleSampleViewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobscheduleSampleViewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
