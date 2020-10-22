import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CommonLoadingComponent } from './common-loading.component';

describe('CommonLoadingComponent', () => {
  let component: CommonLoadingComponent;
  let fixture: ComponentFixture<CommonLoadingComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CommonLoadingComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CommonLoadingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
