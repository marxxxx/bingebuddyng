import { ConfirmationDialogComponent } from './../../components/confirmation-dialog/confirmation-dialog.component';
import { ConfirmationDialogArgs } from './../../components/confirmation-dialog/ConfirmationDialogArgs';
import { UserService } from 'src/app/services/user.service';
import { VenueModel } from 'src/models/VenueModel';
import { VenueDialogArgs } from './../../components/venue-dialog/VenueDialogArgs';
import { VenueDialogComponent } from './../../components/venue-dialog/venue-dialog.component';
import { VenueService } from './../../services/venue.service';
import { MessageDialogComponent } from './../../components/message-dialog/message-dialog.component';
import { ActivatedRoute } from '@angular/router';
import { DrinkType } from './../../../models/DrinkType';
import { AddDrinkActivityDTO } from './../../../models/AddDrinkActivityDTO';
import { Observable, Subscription, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import { LocationDTO } from '../../../models/LocationDTO';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectorRef, ViewChildren } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { AddMessageActivityDTO } from '../../../models/AddMessageActivityDTO';
import { MatDialog, MatTooltip, MatSnackBar } from '@angular/material';
import { ShellInteractionService } from '../../services/shell-interaction.service';
import { FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';
import { NotificationService } from '../../services/notification.service';
import { trigger, style, transition, animate } from '@angular/animations';
import { DrinkDialogComponent } from '../../components/drink-dialog/drink-dialog.component';
import { DrinkDialogArgs } from '../../components/drink-dialog/DrinkDialogArgs';
import { UserInfo } from './../../../models/UserInfo';
import { User } from './../../../models/User';
import { LocationService } from 'src/app/services/location.service';
import { VenueDialogMode } from 'src/app/components/venue-dialog/VenueDialogMode';
import { VenueDialogResult } from 'src/app/components/venue-dialog/VenueDialogResult';
import { TranslateService } from '@ngx-translate/core';

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
  locationIconId = 'location';
  uploader: FileUploader;
  continuationToken: string = null;
  isInitialLoad = true;
  isBusyUploading = false;
  currentProgress = 0;
  isReloadSpinnerActive = false;
  currentUserInfo: UserInfo;
  currentUser: User;
  DrinkType = DrinkType;
  isCommentOpen = false;
  currentVenue: VenueModel;

  @ViewChild('#activity-container')
  container: any;

  @ViewChildren(MatTooltip)
  tooltips: MatTooltip[];

  constructor(private activityService: ActivityService,
    private shellInteraction: ShellInteractionService,
    private auth: AuthService,
    private notification: NotificationService,
    private venueService: VenueService,
    private userService: UserService,
    public locationService: LocationService,
    private changeRef: ChangeDetectorRef,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private translateService: TranslateService,
    private dialog: MatDialog) { }


  ngOnInit() {

    this.initFileUploader();

    this.load();

    this.subscriptions.push(this.notification.activityReceived$.subscribe(_ => this.load()));
    this.subscriptions.push(this.auth.currentUserProfile$
      .pipe(map(p => (p != null ? { userId: p.sub, userName: p.nickname } : null)))
      .subscribe(p => {
        this.currentUserInfo = p;

        // load user to get venue info
        if (p) {
          this.userService.getUser(p.userId)
            .subscribe(u => {
              console.log('loaded user', u);
              this.currentUser = u;
              this.currentVenue = u.currentVenue;

              // refresh location
              this.locationService.refreshLocation(this.currentVenue);
            }, e => console.error('error loading user', p.userId, e));
        }
      }));


    this.subscriptions.push(this.locationService.locationChanged$.subscribe(_ => {
      console.log('location has changed');

      this.snackBar.open(this.translateService.instant('NewVenue'),
        this.translateService.instant('YesCheckin'), { duration: 3000 }).onAction().subscribe(_ => {
          this.onCheckInVenue(VenueDialogMode.LocationChanged);
        });

    }));
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
          volume: 500
        };
        break;
      }
      case DrinkType.Wine: {
        activity = {
          drinkId: '2',
          drinkType: DrinkType.Wine,
          drinkName: 'Wine',
          alcPrc: 9,
          volume: 125
        };
        break;
      }
      case DrinkType.Shot: {
        activity = {
          drinkId: '3',
          drinkType: DrinkType.Shot,
          drinkName: 'Shot',
          alcPrc: 20,
          volume: 40
        };
        break;
      }
      case DrinkType.Anti: {
        activity = {
          drinkId: '3',
          drinkType: DrinkType.Anti,
          drinkName: 'Anti',
          alcPrc: 0,
          volume: 250
        };
        break;
      }
    }

    activity.location = this.locationService.getCurrentLocation();
    activity.venue = this.currentVenue;

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
        location: this.locationService.getCurrentLocation()
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

  onCheckInVenue(mode: VenueDialogMode = VenueDialogMode.Default) {

    const args: VenueDialogArgs = {
      location: this.locationService.getCurrentLocation(),
      mode: mode
    };

    console.log('venueDialogArgs', args);

    this.dialog.open(VenueDialogComponent, { data: args, width: '90%' })
      .afterClosed().subscribe((result: VenueDialogResult) => {

        switch (result.action) {
          case 'leave': {
            this.onResetVenue();
            break;
          }
          case 'change': {
            this.currentVenue = result.venue;

            // update venue for user in backend
            this.venueService.updateCurrentVenue(this.currentVenue).subscribe(
              _ => {
                console.log('updated current venue', this.currentVenue);
                this.load();
              },
              e => console.error('error updating current venue', e));
          }
        }
      });
  }

  onResetVenue() {

    const args: ConfirmationDialogArgs = {
      icon: 'directions_run',
      title: 'LeaveVenue',
      message: 'ReallyLeaveVenue',
      confirmButtonCaption: 'Leave',
      cancelButtonCaption: 'Cancel'
    };

    this.dialog.open(ConfirmationDialogComponent, { data: args }).afterClosed().subscribe(isConfirmed => {

      if (isConfirmed) {
        this.currentVenue = null;
        this.venueService.resetCurrentVenue().subscribe(
          _ => {
            console.log('reset current venue');
            this.load();
          },
          e => console.error('error resetting current venue', e));
      }
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
      url: this.activityService.getAddImageActivityUrl(this.locationService.getCurrentLocation()),
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
