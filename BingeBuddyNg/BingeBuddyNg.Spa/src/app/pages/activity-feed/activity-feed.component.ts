import { MessageDialogComponent } from './../../components/message-dialog/message-dialog.component';
import { ActivatedRoute } from '@angular/router';
import { DrinkType } from './../../../models/DrinkType';
import { AddDrinkActivityDTO } from './../../../models/AddDrinkActivityDTO';
import { Observable, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { LocationDTO } from '../../../models/LocationDTO';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectorRef, ViewChildren } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AddMessageActivityDTO } from '../../../models/AddMessageActivityDTO';
import { UtilService } from '../../services/util.service';
import { MatDialog, MatTooltip } from '@angular/material';
import { ShellInteractionService } from '../../services/shell-interaction.service';
import { FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';
import { NotificationService } from '../../services/notification.service';
import { trigger, style, transition, animate } from '@angular/animations';
import { DrinkDialogComponent } from '../../components/drink-dialog/drink-dialog.component';
import { DrinkDialogArgs } from '../../components/drink-dialog/DrinkDialogArgs';
import { UserInfo } from './../../../models/UserInfo';

@Component({
  selector: 'app-activity-feed',
  templateUrl: './activity-feed.component.html',
  styleUrls: ['./activity-feed.component.scss'],
  animations: [
    trigger('listActivities', [
      transition('true <=> false', [
        style({ transform: 'translateY(-2%)', opacity: 0 }),
        animate('300ms ease-out', style({ transform: 'translateY(0)', opacity: 1 }))
      ])
    ])
  ]
})

export class ActivityFeedComponent implements OnInit, OnDestroy {
  activitys: ActivityStatsDTO[] = [];
  subscriptions: Subscription[] = [];
  isBusy = false;
  isBusyAdding = false;
  location: LocationDTO;
  locationIconId = 'location';
  uploader: FileUploader;
  continuationToken: string = null;
  isInitialLoad = true;
  isBusyUploading = false;
  currentProgress = 0;
  isReloadSpinnerActive = false;
  currentUser: UserInfo;
  DrinkType = DrinkType;
  isCommentOpen = false;

  @ViewChild('#activity-container')
  container: any;

  @ViewChildren(MatTooltip)
  tooltips: MatTooltip[];

  constructor(private activityService: ActivityService,
    private util: UtilService,
    private shellInteraction: ShellInteractionService,
    private auth: AuthService,
    private notification: NotificationService,
    private changeRef: ChangeDetectorRef,
    private route: ActivatedRoute,
    private dialog: MatDialog) { }


  ngOnInit() {

    this.initFileUploader();

    this.load();

    this.subscriptions.push(this.notification.activityReceived$.subscribe(_ => this.load()));
    this.subscriptions.push(this.auth.currentUserProfile$
      .pipe(map(p => (p != null ? { userId: p.sub, userName: p.nickname } : null)))
      .subscribe(p => this.currentUser = p));

    this.route.paramMap.subscribe(_ => {
      console.log('refreshing location');
      this.util.getLocation().then(l => {
        this.location = l;
      }, e => {
        console.error('error retrieving location');
        console.error(e);
      });
    });

  }

  onAppear(ev) {

    if (ev.visible && this.continuationToken && this.continuationToken !== 'null') {
      console.log('loading next page');
      this.load(this.continuationToken);
    }
  }
  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  initFileUploader(): void {
    this.uploader = new FileUploader(this.getOptions());

    this.uploader.onAfterAddingFile = this.onAfterAddingFile.bind(this);
    this.uploader.onCompleteAll = this.onCompleteAll.bind(this);
    this.uploader.onProgressAll = this.onProgressUploading.bind(this);
  }


  load(continuationToken: string = null) {
    console.log('loading activities');
    console.log(continuationToken);
    this.isBusy = true;
    this.activityService.getActivityFeed(continuationToken).subscribe(d => {
      this.continuationToken = d.continuationToken;

      if (continuationToken) {
        this.activitys.push(...d.resultPage);

      } else {
        this.activitys = d.resultPage;
      }
      this.isBusy = false;

      this.isInitialLoad = false;

      // adds reload-spinner after a second so it doesn't interfere with slide-in animation
      setTimeout(() => this.isReloadSpinnerActive = true, 3000);

    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  displayDrinkDialog(type: DrinkType): Observable<any> {
    const args: DrinkDialogArgs = { drinkType: type };
    return this.dialog.open(DrinkDialogComponent, { data: args, width: '90%' }).afterClosed();
  }


  onAddDrink(drinkType: DrinkType) {
    this.displayDrinkDialog(drinkType).subscribe(_ => {
      this.isBusyAdding = true;
      const activity = this.getDrinkActivity(drinkType);
      this.activityService.addDrinkActivity(activity).subscribe(r => {
        this.isBusyAdding = false;
        this.load();
      }, e => {
        this.isBusyAdding = false;
        this.shellInteraction.showErrorMessage();
      });
    });
  }

  getDrinkActivity(type: DrinkType): AddDrinkActivityDTO {
    let activity: AddDrinkActivityDTO = null;
    switch (type) {
      case DrinkType.Beer: {
        activity = {
          drinkId: '1',
          drinkType: DrinkType.Beer,
          drinkName: 'Beer',
          alcPrc: 5,
          volume: 500,
          location: this.location
        };
        break;
      }
      case DrinkType.Wine: {
        activity = {
          drinkId: '2',
          drinkType: DrinkType.Wine,
          drinkName: 'Wine',
          alcPrc: 9,
          volume: 125,
          location: this.location
        };
        break;
      }
      case DrinkType.Shot: {
        activity = {
          drinkId: '3',
          drinkType: DrinkType.Shot,
          drinkName: 'Shot',
          alcPrc: 20,
          volume: 40,
          location: this.location
        };
        break;
      }
      case DrinkType.Anti: {
        activity = {
          drinkId: '3',
          drinkType: DrinkType.Anti,
          drinkName: 'Anti',
          alcPrc: 0,
          volume: 250,
          location: this.location
        };
        break;
      }
    }

    return activity;
  }

  onAddMessage() {
    this.dialog.open(MessageDialogComponent, { width: '80%' }).afterClosed().subscribe(message => {

      if (!message) {
        return;
      }

      this.isBusyAdding = true;
      const activity: AddMessageActivityDTO = {
        message: message,
        location: this.location
      };

      this.activityService.addMessageActivity(activity).subscribe(r => {
        this.isBusyAdding = false;
        this.load();
      }, e => {
        this.isBusyAdding = false;
        this.shellInteraction.showErrorMessage();
      });
    });
  }


  onAfterAddingFile(fileItem: FileItem) {
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

  onCompleteAll() {
    this.uploader.clearQueue();
    this.isBusyAdding = false;
    this.isBusyUploading = false;
    this.currentProgress = 0;
    this.load();
  }

  onProgressUploading(progress: number) {
    this.isBusyAdding = false;
    this.isBusyUploading = true;
    this.currentProgress = progress;
    this.changeRef.detectChanges();
  }

  onActionsOpenChange(open: boolean): void {
    console.log('actions open change', open);
    if (open) {
      setTimeout(() => this.tooltips.forEach(t => t.show()), 500);
    } else {
      this.tooltips.forEach(t => t.hide());
    }

  }
}
