import { TranslocoService } from '@ngneat/transloco';
import { PersonalUsagePerWeekdayDTO } from './../../../../models/PersonalUsagePerWeekdayDTO';
import { Component, OnInit, Input, SimpleChanges, OnChanges } from '@angular/core';

@Component({
  selector: 'app-usage-per-weekday-chart',
  templateUrl: './usage-per-weekday-chart.component.html',
  styleUrls: ['./usage-per-weekday-chart.component.css']
})
export class UsagePerWeekdayChartComponent implements OnInit, OnChanges {

  @Input()
  data: PersonalUsagePerWeekdayDTO[];

  public lineChartData: Array<any> = [
    { data: [], label: '' },
    { data: [], label: '' }
  ];
  public lineChartLabels: Array<any> = [];
  public lineChartOptions: any = {
    responsive: true,
    scales: {
      xAxes: [{
        ticks: {
          autoSkip: false
        },
        scaleLabel: {
          display: true,
          labelString: 'Weekday'
        }
      }],
      yAxes: [
        {
          id: 'prob',
          ticks: {
            min: 0
          },
          scaleLabel: {
            display: true,
            labelString: '%'
          }
        }, {
          id: 'alc',
          ticks: {
            min: 0
          },
          scaleLabel: {
            display: true,
            labelString: '‰'
          }
        }
      ]
    },
  };
  public lineChartColors: Array<any> = [
  ];


  constructor(private translocoService: TranslocoService) { }

  ngOnInit(): void {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.data) {
      this.loadChart(this.data);
    }
  }

  loadChart(data: PersonalUsagePerWeekdayDTO[]): void {
    /*
        const today = new Date().toString().toLowerCase();
        const weekDays = data.map(d => d.weekDay.toLowerCase());
        const todaysWeekDay = weekDays.find(d => today.indexOf(d) >= 0);
    */

    this.lineChartData[0].data = data.map(d => Math.round(d.probability * 10) / 10);
    this.lineChartData[0].label = this.translocoService.translate('Probability');
    this.lineChartData[0].yAxisID = 'prob';

    this.lineChartData[1].data = data.map(d => d.medianMaxAlcLevel);
    this.lineChartData[1].label = this.translocoService.translate('MedianMaxAlcLevel');
    this.lineChartData[1].yAxisID = 'alc';

    this.lineChartLabels = data.map(d => d.weekDay);
    // this.lineChartLabels = data.map(d => d.weekDay);

    // this.lineChartColors =
    //   data.map(d =>
    //     ({
    //       backgroundColor: this.isToday(d.weekDay, todaysWeekDay) ? 'rgba(220, 50, 33,0.2)' : 'rgba(136, 181, 33,0.2)',
    //       borderColor: this.isToday(d.weekDay, todaysWeekDay) ? 'rgba(220, 50, 33,0.2)' : 'rgba(136, 181, 33,0.2)',
    //       pointBackgroundColor: 'rgba(136, 181, 33,1)',
    //       pointBorderColor: '#fff',
    //       pointHoverBackgroundColor: '#fff',
    //       pointHoverBorderColor: 'rgba(136, 181, 33,0.8)'
    //     }));
  }

  private isToday(w1: string, w2: string): boolean {
    const result = w1.toLowerCase() === w2.toLowerCase();
    return result;
  }
}
