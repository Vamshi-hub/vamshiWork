import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SpinnerDlgComponent } from './spinner-dlg.component';

describe('SpinnerDlgComponent', () => {
  let component: SpinnerDlgComponent;
  let fixture: ComponentFixture<SpinnerDlgComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SpinnerDlgComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SpinnerDlgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
