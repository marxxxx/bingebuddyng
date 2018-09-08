import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { ActivityAggregationDTO } from '../../../../models/ActivityAggregationDTO';
import * as moment from 'moment';
import { TranslateService } from '@ngx-translate/core';
import { DefaultChartColors } from '../DefaultChartColors';

@Component({
  selector: 'app-drink-chart',
  templateUrl: './drink-chart.component.html',
  styleUrls: ['./drink-chart.component.css']
})
export class DrinkChartComponent implements OnInit, OnChanges {

  @Input()
  activities: ActivityAggregationDTO[];

  @Input()
  isLegendVisible: boolean;

  // lineChart
  public lineChartData: Array<any> = [
    { data: [], label: '' },
    { data: [], label: '' },
    { data: [], label: '' },
    { data: [], label: '' },
    { data: [], label: '' },
    { data: [], label: '' }
  ];
  public lineChartLabels: Array<any> = [];
  public lineChartOptions: any = {
    responsive: true,
    scales: {
      xAxes: [{
        ticks: {
          autoSkip: true,
          maxTicksLimit: 10
        },
        scaleLabel: {
          display: true,
          labelString: 'Day'
        }
      }],
      yAxes: [
        {
          ticks: {
            min: 0
          },
          scaleLabel: {
            display: true,
            labelString: 'Count'
          }
        }
      ]
    },
  };
  public lineChartColors: Array<any> = [
    { // Total Drinks
      backgroundColor: 'rgba(136, 181, 33,0.2)',
      borderColor: 'rgba(136, 181, 33,1)',
      pointBackgroundColor: 'rgba(136, 181, 33,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(136, 181, 33,0.8)'
    }, ...DefaultChartColors
  ];


  constructor(private translateService: TranslateService) { }

  ngOnInit() {

  }

  ngOnChanges() {
    if (this.activities) {
      this.translateService.get(['Total', 'Beer', 'Wine', 'Shot', 'Anti', 'AlcoholicDrinks']).subscribe(trans => {

        console.log(trans);
        this.activities.forEach(l => this.lineChartLabels.push(moment(l.day).format('DD.MM.YYYY')));

        this.lineChartData = [{
          data: this.activities.map(x => x.count),
          label: trans['Total']
        }, {
          data: this.activities.map(x => x.countBeer),
          label: trans['Beer']
        }, {
          data: this.activities.map(x => x.countWine),
          label: trans['Wine']
        }, {
          data: this.activities.map(x => x.countShots),
          label: trans['Shot']
        }, {
          data: this.activities.map(x => x.countAnti),
          label: trans['Anti']
        }, {
          data: this.activities.map(x => x.countAlc),
          label: trans['AlcoholicDrinks']
        }
        ];
      });

    }
  }

}
