import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardAlecComponent } from './dashboard-alec.component';

describe('DashboardConsultantComponent', () => {
  let component: DashboardAlecComponent;
  let fixture: ComponentFixture<DashboardAlecComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardAlecComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardAlecComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
