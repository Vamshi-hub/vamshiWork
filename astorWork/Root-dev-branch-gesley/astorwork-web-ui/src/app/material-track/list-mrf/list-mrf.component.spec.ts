import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ListMrfComponent } from './list-mrf.component';

describe('ListMrfComponent', () => {
  let component: ListMrfComponent;
  let fixture: ComponentFixture<ListMrfComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ListMrfComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ListMrfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
