import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateMrfComponent } from './create-mrf.component';

describe('CreateMrfComponent', () => {
  let component: CreateMrfComponent;
  let fixture: ComponentFixture<CreateMrfComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateMrfComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateMrfComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
