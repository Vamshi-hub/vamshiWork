import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ShowQrcodeComponent } from './show-qrcode.component';

describe('ShowQrcodeComponent', () => {
  let component: ShowQrcodeComponent;
  let fixture: ComponentFixture<ShowQrcodeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ShowQrcodeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ShowQrcodeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
