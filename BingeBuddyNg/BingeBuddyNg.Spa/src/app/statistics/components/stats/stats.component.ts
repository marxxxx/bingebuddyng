import { ShellInteractionService } from 'src/app/core/services/shell-interaction.service';
import { StatisticsService } from './../../services/statistics.service';
import { Subscription, forkJoin, combineLatest } from 'rxjs';
import { ActivityService } from '../../../activity/services/activity.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ActivityAggregationDTO } from 'src/models/ActivityAggregationDTO';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { AuthService } from 'src/app/core/services/auth.service';
import { UserProfile } from 'src/models/UserProfile';
import { UserStatisticHistoryDTO } from '../../services/UserStatisticHistoryDTO';
import { filter, tap } from 'rxjs/operators';

@Component({
  selector: 'app-stats',
  templateUrl: './stats.component.html',
  styleUrls: ['./stats.component.css']
})
export class StatsComponent implements OnInit, OnDestroy {
  userStatsHistory: UserStatisticHistoryDTO[];
  isBusy = false;
  avgDrinksPerDay = 0;
  isLegendVisible = true;
  userProfile: UserProfile;

  private subscriptions: Subscription[] = [];
  activities: ActivityAggregationDTO[];

  constructor(private route: ActivatedRoute, private activityService: ActivityService,
    private statisticsService: StatisticsService, private authService: AuthService,
    private shellInteraction: ShellInteractionService,
    private media: MediaObserver) { }

  ngOnInit() {

    const sub = combineLatest([this.authService.currentUserProfile$, this.route.params])
      .pipe(filter(r => r[0] != null))
      .pipe(tap(r => this.userProfile = r[0]))
      .subscribe(() => this.load());

    const mediaSubscription = this.media.media$.subscribe((change: MediaChange) => {
      console.log('media change');
      console.log(change);
      if (change.mqAlias === 'xs') {
        this.isLegendVisible = false;
      } else {
        this.isLegendVisible = true;
      }
    });

    this.subscriptions.push(sub, mediaSubscription);
  }

  load(): void {
    this.isBusy = true;

    forkJoin([this.activityService.getActivityAggregation(),
    this.statisticsService.getStatisticsForUser(this.userProfile.sub)])
      .subscribe(r => {
        const activities = r[0];
        this.userStatsHistory = r[1];

        this.isBusy = false;
        this.activities = activities;

        // calculate avg drinks per day
        let sum = 0;
        activities.forEach(d => (sum += d.countAlc));
        this.avgDrinksPerDay = Math.round((sum / activities.length) * 100) / 100;
      }, e => {
        this.isBusy = false;
        this.shellInteraction.showErrorMessage();
      });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }
}
