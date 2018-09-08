import { TranslateService } from '@ngx-translate/core';
import { Subject, Subscription } from 'rxjs';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ActivityAggregationDTO } from '../../../models/ActivityAggregationDTO';


@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit, OnDestroy {

  isBusy = false;
  avgDrinksPerDay = 0;



  private subscriptions: Subscription[] = [];
  activities: ActivityAggregationDTO[];

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


    this.activityService.getActivityAggregation().subscribe(a => {
      this.isBusy = false;
      this.activities = a;

      // calculate avg drinks per day
      let sum = 0;
      a.forEach(d => sum += d.countAlc);
      this.avgDrinksPerDay = Math.round((sum / a.length) * 100) / 100;

    }, e => {
      console.error(e);
      this.isBusy = false;
    });

  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

}
