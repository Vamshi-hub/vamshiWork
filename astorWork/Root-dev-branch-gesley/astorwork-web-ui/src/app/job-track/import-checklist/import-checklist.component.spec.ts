import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportchecklistComponent } from './import-checklist.component';

describe('ImportchecklistComponent', () => {
  let component: ImportchecklistComponent;
  let fixture: ComponentFixture<ImportchecklistComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportchecklistComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportchecklistComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
