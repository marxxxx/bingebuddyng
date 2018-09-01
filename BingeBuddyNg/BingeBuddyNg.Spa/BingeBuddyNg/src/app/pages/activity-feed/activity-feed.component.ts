import { DrinkType } from './../../../models/DrinkType';
import { AddDrinkActivityDTO } from './../../../models/AddDrinkActivityDTO';
import { Observable, Subscription } from 'rxjs';
import { AddActivityBaseDTO } from './../../../models/AddActivityBaseDTO';
import { TranslateService } from '@ngx-translate/core';
import { LocationDTO } from '../../../models/LocationDTO';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { ActivityService } from '../../services/activity.service';
import { DataService } from '../../services/data.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AddMessageActivityDTO } from '../../../models/AddMessageActivityDTO';
import { UtilService } from '../../services/util.service';
import { MatSnackBar } from '@angular/material';
import { ShellInteractionService } from '../../services/shell-interaction.service';
import { FileUploader, FileItem, FileUploaderOptions, ParsedResponseHeaders } from 'ng2-file-upload';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-activity-feed',
  templateUrl: './activity-feed.component.html',
  styleUrls: ['./activity-feed.component.scss']
})
export class ActivityFeedComponent implements OnInit, OnDestroy {
  activitys: ActivityStatsDTO[] = [];
  subscriptions: Subscription[] = [];
  isBusy = false;
  isBusyAdding = false;
  location: LocationDTO;
  locationIconId = 'location';
  uploader: FileUploader;

  constructor(private activityService: ActivityService,
    private util: UtilService,
    private shellInteraction: ShellInteractionService,
    private auth: AuthService,
    private notification: NotificationService,
    private snackBar: MatSnackBar,
    private transate: TranslateService) { }

  ngOnInit() {

    this.initFileUploader();

    this.load();

    const sub = this.notification.activityReceived$.subscribe( _ => this.load());
    this.subscriptions.push(sub);

    this.shellInteraction.addShellIcon({ id: this.locationIconId, name: 'not_listed_location', tooltip: 'QueryingLocation' });

    this.util.getLocation().then(l => {
      this.location = l;
      this.shellInteraction.addShellIcon({ id: this.locationIconId, name: 'location_on', tooltip: `${l.longitude} / ${l.latitude}` });
    }, e => {
      // this.snackBar.open(this.transate.instant('NoGeolocationMessage'), 'OK', { duration: 3000 });
      this.shellInteraction.addShellIcon({ id: this.locationIconId, name: 'location_off', tooltip: 'NoGeolocationMessage' });
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  initFileUploader(): void {
    this.uploader = new FileUploader(this.getOptions());

    this.uploader.onAfterAddingFile = this.onAfterAddingFile.bind(this);
    this.uploader.onCompleteAll = this.onCompleteAll.bind(this);
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


  onAfterAddingFile(fileItem: FileItem) {

    // update options to reflect deviceId in upload url
    this.uploader.setOptions(this.getOptions());

    // upload
    this.isBusyAdding = true;
    this.uploader.uploadAll();
  }

  getOptions(): FileUploaderOptions {

    const options = {
      url: this.activityService.getAddImageActivityUrl(this.location),
      authTokenHeader: 'Authorization',
      authToken: 'Bearer ' + this.auth.getAccessToken()
    };

    return options;
  }

  // onCompleteItem(item: FileItem, response: string, status: number, headers: ParsedResponseHeaders) {
  //   console.log('upload completed');
  //   console.log(response);
  //   this.isBusyAdding = false;
  // }

  onCompleteAll() {
    this.uploader.clearQueue();
    this.isBusyAdding = false;
    this.load();
  }
}
