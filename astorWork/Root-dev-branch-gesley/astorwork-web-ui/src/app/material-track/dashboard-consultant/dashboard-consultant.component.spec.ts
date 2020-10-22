import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardConsultantComponent } from './dashboard-consultant.component';

describe('DashboardConsultantComponent', () => {
  let component: DashboardConsultantComponent;
  let fixture: ComponentFixture<DashboardConsultantComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardConsultantComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardConsultantComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
