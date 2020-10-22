import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialstagedashboardComponent } from './materialstagedashboard.component';

describe('MaterialstagedashboardComponent', () => {
  let component: MaterialstagedashboardComponent;
  let fixture: ComponentFixture<MaterialstagedashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialstagedashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialstagedashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
