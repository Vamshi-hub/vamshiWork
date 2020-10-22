import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OrganisationMasterComponent } from './organisation-master.component';

describe('OrganisationMasterComponent', () => {
  let component: OrganisationMasterComponent;
  let fixture: ComponentFixture<OrganisationMasterComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OrganisationMasterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OrganisationMasterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
