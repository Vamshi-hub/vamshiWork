import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ChangePwDlgComponent } from './result-dlg.component';

describe('ChangePwDlgComponent', () => {
  let component: ChangePwDlgComponent;
  let fixture: ComponentFixture<ChangePwDlgComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ChangePwDlgComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ChangePwDlgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
