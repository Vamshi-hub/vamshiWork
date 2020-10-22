import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BimSyncDetailsComponent } from './bim-sync-details.component';

describe('BimSyncDetailsComponent', () => {
  let component: BimSyncDetailsComponent;
  let fixture: ComponentFixture<BimSyncDetailsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BimSyncDetailsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BimSyncDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
