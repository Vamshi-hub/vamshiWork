import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SampleViewFileComponent } from './sample-view.component';

describe('SampleViewFileComponent', () => {
  let component: SampleViewFileComponent;
  let fixture: ComponentFixture<SampleViewFileComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SampleViewFileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SampleViewFileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
