import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialQcComponent } from './material-qc.component';

describe('MaterialQcComponent', () => {
  let component: MaterialQcComponent;
  let fixture: ComponentFixture<MaterialQcComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MaterialQcComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MaterialQcComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
