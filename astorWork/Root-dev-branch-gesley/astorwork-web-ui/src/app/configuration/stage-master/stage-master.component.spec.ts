import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { StageMasterComponent } from './stage-master.component';

describe('StageMasterComponent', () => {
  let component: StageMasterComponent;
  let fixture: ComponentFixture<StageMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ StageMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(StageMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
