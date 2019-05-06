import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DrinkEventService } from 'src/app/core/services/drinkevent.service';
import { DrinkEvent } from 'src/models/DrinkEvent';
import * as moment from 'moment';
import { MatTooltip } from '@angular/material';
import { AuthService } from 'src/app/core/services/auth.service';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-drink-event-counter',
  templateUrl: './drink-event-counter.component.html',
  styleUrls: ['./drink-event-counter.component.css']
})
export class DrinkEventCounterComponent implements OnInit, OnDestroy {
  remainingTime: string;
  currentDrinkEvent: DrinkEvent;
  intervalId: any;
  subscriptions: Subscription[] = [];

  @ViewChild(MatTooltip)
  tooltips: MatTooltip;

  constructor(private authService: AuthService, private drinkEventService: DrinkEventService) { }

  ngOnInit() {

    this.subscriptions.push(this.authService.isLoggedIn$.pipe(filter(isLoggedIn => isLoggedIn))
      .subscribe(_ => {
        this.load();
      }));

    this.subscriptions.push(this.drinkEventService.currentUserScored$.subscribe(_ => this.currentDrinkEvent = null));
  }

  isVisible(): boolean {

    if (!this.currentDrinkEvent) {
      return false;
    }

    const currentUserAlreadyScored = this.currentDrinkEvent.scoringUserIds &&
      this.currentDrinkEvent.scoringUserIds.includes(this.authService.currentUserProfile$.value.sub);
    if (currentUserAlreadyScored === true) {
      return false;
    }

    return true;
  }

  load() {
    this.drinkEventService.getCurrentDrinkEvent().subscribe(
      r => {
        this.currentDrinkEvent = r;
        if (this.isVisible()) {
          this.startCounter();
        }
      },
      e => {
        console.error(
          'DrinkEventCounterComponent: Error loading drink event info',
          e
        );
      }
    );
  }

  ngOnDestroy() {
    this.stopCounter();

    this.subscriptions.forEach(s => s.unsubscribe());
  }

  startCounter(): any {
    this.updateRemainingTime();

    this.intervalId = setInterval(() => this.updateRemainingTime(), 1000);
  }

  stopCounter() {
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  updateRemainingTime() {
    const remainingSeconds = moment(this.currentDrinkEvent.endUtc).diff(
      moment(),
      'seconds'
    );
    if (remainingSeconds < 0) {
      this.stopCounter();
    } else {
      const duration = moment.duration(remainingSeconds, 'seconds');
      this.remainingTime = duration.minutes() + ':' + duration.seconds();

      console.log('DrinkEventCounter: remaining time', this.remainingTime);
    }
  }
}
