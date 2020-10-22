import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobQCComponent } from './job-qc.component';

describe('JobQCComponent', () => {
  let component: JobQCComponent;
  let fixture: ComponentFixture<JobQCComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobQCComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobQCComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
