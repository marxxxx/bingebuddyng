import { TranslocoService } from '@ngneat/transloco';
import { UserStatisticHistoryDTO } from './../../services/UserStatisticHistoryDTO';
import { Component, OnInit, Input, SimpleChanges, OnChanges } from '@angular/core';
import { format } from 'date-fns';

@Component({
  selector: 'app-alc-history-chart',
  templateUrl: './alc-history-chart.component.html',
  styleUrls: ['./alc-history-chart.component.css']
})
export class AlcHistoryChartComponent implements OnInit, OnChanges {

  @Input()
  userStatsHistory: UserStatisticHistoryDTO[];

  // lineChart
  public lineChartData: Array<any> = [
    { data: [], label: '‰' }
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
          labelString: 'Time'
        }
      }],
      yAxes: [
        {
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
    {
      backgroundColor: 'rgba(136, 181, 33,0.2)',
      borderColor: 'rgba(136, 181, 33,1)',
      pointBackgroundColor: 'rgba(136, 181, 33,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(136, 181, 33,0.8)'
    }
  ];


  constructor(private trans: TranslocoService) { }

  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    if (this.userStatsHistory) {
      this.userStatsHistory.forEach(l => this.lineChartLabels.push(format(new Date(l.timestamp), 'HH:mm')));
      this.lineChartData = [{
        data: this.userStatsHistory.map(x => x.alcLevel),
        label: '‰'
      }];
    }
  }

}
