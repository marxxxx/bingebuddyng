import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AlcHistoryChartComponent } from './alc-history-chart.component';

describe('AlcHistoryChartComponent', () => {
  let component: AlcHistoryChartComponent;
  let fixture: ComponentFixture<AlcHistoryChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AlcHistoryChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AlcHistoryChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
