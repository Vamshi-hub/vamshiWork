import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PowerbiViewerComponent } from './powerbi-viewer.component';

describe('PowerbiViewerComponent', () => {
  let component: PowerbiViewerComponent;
  let fixture: ComponentFixture<PowerbiViewerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PowerbiViewerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PowerbiViewerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
