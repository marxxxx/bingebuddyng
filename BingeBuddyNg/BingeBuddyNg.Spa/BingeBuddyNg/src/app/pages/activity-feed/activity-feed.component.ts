import { TranslateService } from '@ngx-translate/core';
import { LocationDTO } from '../../../models/LocationDTO';
import { ActivityDTO } from '../../../models/ActivityDTO';
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
  styleUrls: ['./activity-feed.component.css']
})
export class ActivityFeedComponent implements OnInit {
  activitys: ActivityDTO[] = [];
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


    this.shellInteraction.addShellIcon({id: this.locationIconId, name: 'not_listed_location', tooltip: 'QueryingLocation'});

    this.util.getLocation().then(l => {
      this.location = l;
      this.shellInteraction.addShellIcon({id: this.locationIconId, name: 'location_on', tooltip: `${l.longitude} / ${l.latitude}`});
    }, e => {
      // this.snackBar.open(this.transate.instant('NoGeolocationMessage'), 'OK', { duration: 3000 });
      this.shellInteraction.addShellIcon({id: this.locationIconId, name: 'location_off', tooltip: 'NoGeolocationMessage'});
    });
  }

  load() {
    this.isBusy = true;
    this.activityService.getActivitys({onlyWithLocation: false}).subscribe(d => {
      this.activitys = d;
      this.isBusy = false;
      console.log('got data');
      console.log(d);
    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  onAddActivity() {
    this.isBusyAdding = true;
    const activity: AddMessageActivityDTO = {
      message: 'I had a drink! Cheers!',
      location: this.location
    };
    this.activityService.addActivity(activity).subscribe(r => {
      this.isBusyAdding = false;
      this.load();
    }, e => {
      this.isBusyAdding = false;
    });
  }

}
