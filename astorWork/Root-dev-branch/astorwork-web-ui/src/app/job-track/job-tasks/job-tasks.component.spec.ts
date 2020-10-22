import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { JobTasksComponent } from './job-tasks.component';

describe('JobTasksComponent', () => {
  let component: JobTasksComponent;
  let fixture: ComponentFixture<JobTasksComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobTasksComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobTasksComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
