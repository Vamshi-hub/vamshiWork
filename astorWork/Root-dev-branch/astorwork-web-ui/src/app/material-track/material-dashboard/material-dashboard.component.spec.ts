import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialDashboardComponent } from './material-dashboard.component';

describe('MaterialDashboardComponent', () => {
  let component: MaterialDashboardComponent;
  let fixture: ComponentFixture<MaterialDashboardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
