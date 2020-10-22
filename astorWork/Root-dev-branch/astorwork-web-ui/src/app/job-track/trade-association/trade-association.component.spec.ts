import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeAssociationComponent} from './trade-association.component';

describe('TradeAssociationComponent', () => {
  let component: TradeAssociationComponent;
  let fixture: ComponentFixture<TradeAssociationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradeAssociationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeAssociationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
