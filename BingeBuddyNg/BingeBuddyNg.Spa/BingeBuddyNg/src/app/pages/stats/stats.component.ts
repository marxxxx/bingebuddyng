import { TranslateService } from '@ngx-translate/core';
import { Subject, Subscription } from 'rxjs';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import * as moment from 'moment';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit, OnDestroy {

  isBusy = false;

  // lineChart
  public lineChartData: Array<any> = [
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
            labelString: 'Drinks'
          }
        }
      ]
    },
  };
  public lineChartColors: Array<any> = [
    { // grey
      backgroundColor: 'rgba(236,132,39,0.2)',
      borderColor: 'rgba(236,132,39,1)',
      pointBackgroundColor: 'rgba(236,132,39,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(236,132,39,0.8)'
    }
  ];
  public lineChartLegend = true;
  public lineChartType = 'line';

  private subscriptions: Subscription[] = [];

  constructor(private route: ActivatedRoute, private activityService: ActivityService,
    private translateService: TranslateService) {
  }

  ngOnInit() {
    const routeSubscription = this.route.params.subscribe(r => {
      this.load();
    });

    this.subscriptions.push(routeSubscription);

  }

  load(): void {

    this.isBusy = true;
    const drinksLabel = this.translateService.instant('Drinks');

    this.activityService.getActivityAggregation().subscribe(a => {
      this.isBusy = false;
      a.forEach(l => this.lineChartLabels.push(moment(l.day).format('DD.MM.YYYY')));

      this.lineChartData = [{
        data: a.map(x => x.count),
        label: drinksLabel
      }
      ];
    }, e => {
      console.error(e);
      this.isBusy = false;
    });

  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

}
