import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UsagePerWeekdayChartComponent } from './usage-per-weekday-chart.component';

describe('UsagePerWeekdayChartComponent', () => {
  let component: UsagePerWeekdayChartComponent;
  let fixture: ComponentFixture<UsagePerWeekdayChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UsagePerWeekdayChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UsagePerWeekdayChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
