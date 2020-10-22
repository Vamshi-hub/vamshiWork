import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ImportMaterialComponent } from './import-material.component';

describe('ImportMaterialComponent', () => {
  let component: ImportMaterialComponent;
  let fixture: ComponentFixture<ImportMaterialComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ImportMaterialComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ImportMaterialComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
