import { Component, OnInit, Input, OnDestroy, ViewChild } from '@angular/core';
import { DrinkEventService } from 'src/app/services/drinkevent.service';
import { DrinkEvent } from 'src/models/DrinkEvent';
import * as moment from 'moment';
import { MatTooltip } from '@angular/material';

@Component({
  selector: 'app-drink-event-counter',
  templateUrl: './drink-event-counter.component.html',
  styleUrls: ['./drink-event-counter.component.css']
})
export class DrinkEventCounterComponent implements OnInit, OnDestroy {
  remainingTime: string;
  currentDrinkEvent: DrinkEvent;
  intervalId: any;

  @ViewChild(MatTooltip)
  tooltips: MatTooltip;

  constructor(private drinkEventService: DrinkEventService) {}

  ngOnInit() {
    this.drinkEventService.getCurrentDrinkEvent().subscribe(
      r => {
        this.currentDrinkEvent = r;
        if (this.currentDrinkEvent != null) {
          console.log('DrinkEventCounterComponent: Active drink event detected! Starting counter.')
          this.startCounter();
        } else {
          console.log('DrinkEventCounterComponent: No active drink event.')
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
    if (this.intervalId) {
      clearInterval(this.intervalId);
      this.intervalId = null;
    }
  }

  startCounter(): any {
    this.updateRemainingTime();

    this.intervalId = setInterval(() => this.updateRemainingTime(), 1000);
  }

  updateRemainingTime() {
    const remainingSeconds = moment(this.currentDrinkEvent.endUtc).diff(moment(), 'seconds');
    const duration = moment.duration(remainingSeconds, 'seconds');
    this.remainingTime = duration.minutes() + ':' + duration.seconds();

    console.log('DrinkEventCounter: remaining time', this.remainingTime);
  }
}
