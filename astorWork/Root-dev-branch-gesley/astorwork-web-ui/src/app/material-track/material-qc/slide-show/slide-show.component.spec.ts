import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialSlideShowComponent } from './slide-show.component';


describe('MaterialSlideShowComponent', () => {
  let component: MaterialSlideShowComponent;
  let fixture: ComponentFixture<MaterialSlideShowComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialSlideShowComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialSlideShowComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});


