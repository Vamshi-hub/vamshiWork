import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListDefectComponent } from './list-defect.component';

describe('ListDefectComponent', () => {
  let component: ListDefectComponent;
  let fixture: ComponentFixture<ListDefectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListDefectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListDefectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
