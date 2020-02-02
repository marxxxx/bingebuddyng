import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { DrinkEventService } from 'src/app/@core/services/drinkevent.service';
import { DrinkEvent } from 'src/models/DrinkEvent';
import { MatTooltip } from '@angular/material/tooltip';
import { AuthService } from 'src/app/@core/services/auth.service';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';
import { NotificationService } from '../../services/notification.service';

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

  @ViewChild(MatTooltip, { static: false })
  tooltips: MatTooltip;

  constructor(
    private authService: AuthService,
    private drinkEventService: DrinkEventService,
    private notificationService: NotificationService
  ) { }

  ngOnInit() {
    this.subscriptions.push(
      this.authService.isLoggedIn$.pipe(filter(isLoggedIn => isLoggedIn)).subscribe(_ => {
        this.load();
      })
    );

    this.subscriptions.push(
      this.notificationService.activityReceived$.subscribe(_ => {
        this.load();
      })
    );

    this.subscriptions.push(this.drinkEventService.currentUserScored$.subscribe(_ => (this.currentDrinkEvent = null)));
  }

  isVisible(): boolean {
    if (!this.currentDrinkEvent) {
      return false;
    }

    const currentUserAlreadyScored =
      this.currentDrinkEvent.scoringUserIds &&
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
        console.error('DrinkEventCounterComponent: Error loading drink event info', e);
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
    if (!this.currentDrinkEvent) {
      this.stopCounter();
      return;
    }
    const remainingSeconds = this.calculateDiffInSeconds(new Date(), new Date(this.currentDrinkEvent.endUtc));

    if (remainingSeconds < 0) {
      this.stopCounter();
      this.currentDrinkEvent = null;
    } else {
      this.remainingTime = this.getTwoDigitNumber(Math.floor(remainingSeconds / 60)) +
        ':' +
        this.getTwoDigitNumber(Math.round(remainingSeconds % 60));
    }
  }

  getTwoDigitNumber(num: number): string {
    if (num > 10) {
      return num.toString();
    } else {
      return '0' + num.toString();
    }
  }

  calculateDiffInSeconds(startDate: Date, endDate: Date): number {
    const seconds = (endDate.getTime() - startDate.getTime()) / 1000;
    return Math.round(seconds);
  }
}
