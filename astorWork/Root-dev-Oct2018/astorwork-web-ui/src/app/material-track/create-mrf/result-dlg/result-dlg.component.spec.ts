import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ResultDlgComponent } from './result-dlg.component';

describe('ResultDlgComponent', () => {
  let component: ResultDlgComponent;
  let fixture: ComponentFixture<ResultDlgComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ResultDlgComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ResultDlgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
