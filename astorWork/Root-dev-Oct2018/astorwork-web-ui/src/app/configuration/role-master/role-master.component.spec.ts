import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleMasterComponent } from './role-master.component';

describe('RoleMasterComponent', () => {
  let component: RoleMasterComponent;
  let fixture: ComponentFixture<RoleMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoleMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoleMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
