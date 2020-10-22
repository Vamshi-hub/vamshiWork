import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobSchedulingComponent } from './job-scheduling.component';

describe('JobSchedulingComponent', () => {
  let component: JobSchedulingComponent;
  let fixture: ComponentFixture<JobSchedulingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobSchedulingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobSchedulingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
