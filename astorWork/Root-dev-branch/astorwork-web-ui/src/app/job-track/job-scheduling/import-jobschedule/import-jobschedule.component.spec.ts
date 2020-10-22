import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportJobscheduleComponent } from './import-jobschedule.component';

describe('ImportJobscheduleComponent', () => {
  let component: ImportJobscheduleComponent;
  let fixture: ComponentFixture<ImportJobscheduleComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportJobscheduleComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportJobscheduleComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
