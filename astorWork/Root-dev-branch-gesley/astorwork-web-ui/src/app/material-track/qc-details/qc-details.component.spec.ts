import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { QcDetailsComponent } from './qc-details.component';

describe('QcDetailsComponent', () => {
  let component: QcDetailsComponent;
  let fixture: ComponentFixture<QcDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ QcDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QcDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
