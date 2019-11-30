import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { ActivityAggregationDTO } from '../../../../models/ActivityAggregationDTO';
import { DefaultChartColors } from '../DefaultChartColors';
import { TranslocoService } from '@ngneat/transloco';

@Component({
  selector: 'app-drink-ratio-chart',
  templateUrl: './drink-ratio-chart.component.html',
  styleUrls: ['./drink-ratio-chart.component.css']
})
export class DrinkRatioChartComponent implements OnInit, OnChanges {

  @Input()
  activities: ActivityAggregationDTO[];

  // chart data
  public chartData: Array<any> = [
    { data: [], label: '' }
  ];
  public chartLabels: any = ['Beer', 'Wine', 'Shot', 'Anti'];
  public chartOptions: any = {
    responsive: true
  };
  public chartColors = [{ backgroundColor: DefaultChartColors.map(c => c.borderColor) }];

  constructor(private translateService: TranslocoService) { }

  ngOnInit() {
  }


  ngOnChanges() {
    if (this.activities) {
      this.translateService.selectTranslate(['DrinkRatio', 'Beer', 'Wine', 'Shot', 'Anti']).subscribe(trans => {

        console.log(trans);
        this.chartData = [{
          data: [
            this.calculateSum(this.activities.map(x => x.countBeer)),
            this.calculateSum(this.activities.map(x => x.countWine)),
            this.calculateSum(this.activities.map(x => x.countShots)),
            this.calculateSum(this.activities.map(x => x.countAnti))
          ],
          label: trans['DrinkRatio']
        }];

        this.chartLabels = [trans['Beer'], trans['Wine'], trans['Shot'], trans['Anti']];
      });

    }
  }

  private calculateSum(values: number[]): number {
    let result = 0;
    values.forEach(v => result += v);
    return result;
  }

}
