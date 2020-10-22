import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListBimSyncComponent } from './list-bim-sync.component';

describe('ListBimSyncComponent', () => {
  let component: ListBimSyncComponent;
  let fixture: ComponentFixture<ListBimSyncComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListBimSyncComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListBimSyncComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
