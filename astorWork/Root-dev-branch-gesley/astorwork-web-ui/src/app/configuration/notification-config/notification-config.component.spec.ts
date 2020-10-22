import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NotificationConfigComponent } from './notification-config.component';

describe('NotificationConfigComponent', () => {
  let component: NotificationConfigComponent;
  let fixture: ComponentFixture<NotificationConfigComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NotificationConfigComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NotificationConfigComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
