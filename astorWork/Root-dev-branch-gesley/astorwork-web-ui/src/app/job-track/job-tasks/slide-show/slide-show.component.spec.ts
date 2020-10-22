import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { JobTasksSlideShowComponent } from './slide-show.component';

describe('JobTasksSlideShowComponent', () => {
  let component: JobTasksSlideShowComponent;
  let fixture: ComponentFixture<JobTasksSlideShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobTasksSlideShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobTasksSlideShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
