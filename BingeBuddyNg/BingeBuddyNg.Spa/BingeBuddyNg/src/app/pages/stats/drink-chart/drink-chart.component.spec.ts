import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DrinkChartComponent } from './drink-chart.component';

describe('DrinkChartComponent', () => {
  let component: DrinkChartComponent;
  let fixture: ComponentFixture<DrinkChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DrinkChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DrinkChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
