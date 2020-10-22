import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { JobSlideShowComponent } from './slide-show.component';

describe('JobSlideShowComponent', () => {
  let component: JobSlideShowComponent;
  let fixture: ComponentFixture<JobSlideShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ JobSlideShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(JobSlideShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
