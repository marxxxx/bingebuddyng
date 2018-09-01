import { DrinkType } from './../../../models/DrinkType';
import { AddDrinkActivityDTO } from './../../../models/AddDrinkActivityDTO';
import { Observable } from 'rxjs';
import { AddActivityBaseDTO } from './../../../models/AddActivityBaseDTO';
import { TranslateService } from '@ngx-translate/core';
import { LocationDTO } from '../../../models/LocationDTO';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { ActivityService } from '../../services/activity.service';
import { DataService } from '../../services/data.service';
import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AddMessageActivityDTO } from '../../../models/AddMessageActivityDTO';
import { UtilService } from '../../services/util.service';
import { MatSnackBar } from '@angular/material';
import { ShellInteractionService } from '../../services/shell-interaction.service';

@Component({
  selector: 'app-activity-feed',
  templateUrl: './activity-feed.component.html',
  styleUrls: ['./activity-feed.component.scss']
})
export class ActivityFeedComponent implements OnInit {
  activitys: ActivityStatsDTO[] = [];
  isBusy = false;
  isBusyAdding = false;
  location: LocationDTO;
  locationIconId = 'location';

  constructor(private activityService: ActivityService,
    private util: UtilService,
    private shellInteraction: ShellInteractionService,
    private snackBar: MatSnackBar,
    private transate: TranslateService) { }

  ngOnInit() {
    this.load();


    this.shellInteraction.addShellIcon({ id: this.locationIconId, name: 'not_listed_location', tooltip: 'QueryingLocation' });

    this.util.getLocation().then(l => {
      this.location = l;
      this.shellInteraction.addShellIcon({ id: this.locationIconId, name: 'location_on', tooltip: `${l.longitude} / ${l.latitude}` });
    }, e => {
      // this.snackBar.open(this.transate.instant('NoGeolocationMessage'), 'OK', { duration: 3000 });
      this.shellInteraction.addShellIcon({ id: this.locationIconId, name: 'location_off', tooltip: 'NoGeolocationMessage' });
    });
  }

  load() {
    this.isBusy = true;
    this.activityService.getActivityFeed().subscribe(d => {
      this.activitys = d;
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }



  onAddBeer() {
    this.isBusyAdding = true;
    const activity: AddDrinkActivityDTO = {
      drinkId: '1',
      drinkType: DrinkType.Beer,
      drinkName: 'Beer',
      alcPrc: 5,
      volume: 500,
      location: this.location
    };

    this.activityService.addDrinkActivity(activity).subscribe(r => {
      this.isBusyAdding = false;
      this.load();
    }, e => {
      this.isBusyAdding = false;
    });
  }

  onAddWine() {
    this.isBusyAdding = true;
    const activity: AddDrinkActivityDTO = {
      drinkId: '2',
      drinkType: DrinkType.Wine,
      drinkName: 'Wine',
      alcPrc: 9,
      volume: 125,
      location: this.location
    };


    this.activityService.addDrinkActivity(activity).subscribe(r => {
      this.isBusyAdding = false;
      this.load();
    }, e => {
      this.isBusyAdding = false;
    });
  }

  onAddShot() {
    this.isBusyAdding = true;
    const activity: AddDrinkActivityDTO = {
      drinkId: '3',
      drinkType: DrinkType.Shot,
      drinkName: 'Shot',
      alcPrc: 20,
      volume: 40,
      location: this.location
    };

    this.activityService.addDrinkActivity(activity).subscribe(r => {
      this.isBusyAdding = false;
      this.load();
    }, e => {
      this.isBusyAdding = false;
    });
  }

  onAddAnti() {
    this.isBusyAdding = true;
    const activity: AddDrinkActivityDTO = {
      drinkId: '3',
      drinkType: DrinkType.Anti,
      drinkName: 'Antialcoholic Drink',
      alcPrc: 0,
      volume: 250,
      location: this.location
    };

    this.activityService.addDrinkActivity(activity).subscribe(r => {
      this.isBusyAdding = false;
      this.load();
    }, e => {
      this.isBusyAdding = false;
    });
  }

  onAddMessage() {
    this.isBusyAdding = true;
    const activity: AddMessageActivityDTO = {
      message: 'Hello this is a message!',
      location: this.location
    };

    this.activityService.addMessageActivity(activity).subscribe(r => {
      this.isBusyAdding = false;
      this.load();
    }, e => {
      this.isBusyAdding = false;
    });
  }
}
