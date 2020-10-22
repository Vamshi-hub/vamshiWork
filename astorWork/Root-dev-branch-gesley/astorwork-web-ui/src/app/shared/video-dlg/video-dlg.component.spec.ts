import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VideoDlgComponent } from './video-dlg.component';

describe('VideoDlgComponent', () => {
  let component: VideoDlgComponent;
  let fixture: ComponentFixture<VideoDlgComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VideoDlgComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VideoDlgComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
