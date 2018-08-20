import { ActivityType } from './../../../models/ActivityType';
import { ActivityDTO } from './../../../models/ActivityDTO';
import { Component, OnInit, Input } from '@angular/core';
import { TranslateService } from '../../../../node_modules/@ngx-translate/core';

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.css']
})
export class ActivityComponent implements OnInit {

  @Input()
  activity: ActivityDTO;

  constructor(private translate: TranslateService) { }

  ngOnInit() {
  }

  isDrinkActivity(): boolean {
    return this.activity.activityType === ActivityType.Drink;
  }

  isMessageActivity(): boolean {
    return this.activity.activityType === ActivityType.Message;
  }

  getDrinkMessage(): string {
    if (!this.activity.drinkName) {
      return this.activity.message;
    }
    const message = this.translate.instant('IJustHadA', { value: this.translate.instant(this.activity.drinkName) });
    return message;
  }
}
