import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DashboardMainconComponent } from './dashboard-maincon.component';

describe('DashboardMainconComponent', () => {
  let component: DashboardMainconComponent;
  let fixture: ComponentFixture<DashboardMainconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DashboardMainconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DashboardMainconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
