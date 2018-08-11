import { Subject, Subscription } from 'rxjs';
import { ActivityService } from './../../services/activity.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit, OnDestroy {

  // lineChart
  public lineChartData: Array<any> = [
    { data: [], label: '' }
  ];
  public lineChartLabels: Array<any> = [];
  public lineChartOptions: any = {
    responsive: true
  };
  public lineChartColors: Array<any> = [
    { // grey
      backgroundColor: 'rgba(148,159,177,0.2)',
      borderColor: 'rgba(148,159,177,1)',
      pointBackgroundColor: 'rgba(148,159,177,1)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(148,159,177,0.8)'
    }
  ];
  public lineChartLegend = true;
  public lineChartType = 'line';

  private subscriptions: Subscription[] = [];

  constructor(private route: ActivatedRoute, private activityService: ActivityService) {
  }

  ngOnInit() {
    const routeSubscription = this.route.params.subscribe(r => {
      this.load();
    });

    this.subscriptions.push(routeSubscription);

  }

  load(): void {

    this.activityService.getActivityAggregation().subscribe(a => {
      this.lineChartLabels = a.map(x => x.day.toString());
      this.lineChartData = [{
        data: a.map(x => x.count),
        label: 'Drinks'
      }
      ];
    });

  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

}
