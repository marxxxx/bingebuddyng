import { DrinkActivityService } from '../../services/drink-activity.service';
import { ConfirmationDialogComponent } from '../../../shared/components/confirmation-dialog/confirmation-dialog.component';
import { ConfirmationDialogArgs } from '../../../shared/components/confirmation-dialog/ConfirmationDialogArgs';
import { UserService } from 'src/app/core/services/user.service';
import { VenueDialogArgs } from '../venue-dialog/VenueDialogArgs';
import { VenueDialogComponent } from '../venue-dialog/venue-dialog.component';
import { MessageDialogComponent } from '../message-dialog/message-dialog.component';
import { DrinkType } from '../../../../models/DrinkType';
import { Subscription } from 'rxjs';
import { map, filter } from 'rxjs/operators';
import { ActivityStatsDTO } from '../../../../models/ActivityStatsDTO';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectorRef, ViewChildren } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { AddMessageActivityDTO } from '../../../../models/AddMessageActivityDTO';
import { MatDialog, MatTooltip, MatSnackBar } from '@angular/material';
import { ShellInteractionService } from '../../../core/services/shell-interaction.service';
import { FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';
import { NotificationService } from '../../../core/services/notification.service';
import { trigger, style, transition, animate } from '@angular/animations';
import { UserInfo } from '../../../../models/UserInfo';
import { User } from '../../../../models/User';
import { LocationService } from 'src/app/activity/services/location.service';
import { VenueDialogMode } from '../venue-dialog/VenueDialogMode';
import { VenueDialogResult } from '../venue-dialog/VenueDialogResult';
import { TranslateService } from '@ngx-translate/core';
import { Drink } from 'src/models/Drink';
import { DrinkRetrieverService } from '../../services/drink-retriever.service';

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
  drinks: Drink[];

  @ViewChild('#activity-container')
  container: any;

  @ViewChildren(MatTooltip)
  tooltips: MatTooltip[];

  constructor(
    private activityService: ActivityService,
    private shellInteraction: ShellInteractionService,
    private auth: AuthService,
    private notification: NotificationService,
    private userService: UserService,
    public locationService: LocationService,
    private drinkActivityService: DrinkActivityService,
    private drinkService: DrinkRetrieverService,
    private changeRef: ChangeDetectorRef,
    private snackBar: MatSnackBar,
    private translateService: TranslateService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.initFileUploader();

    this.subscriptions.push(this.notification.activityReceived$.subscribe(_ => this.load()));
    this.subscriptions.push(
      this.auth.currentUserProfile$
      .pipe(filter(p => p != null))
      .pipe(map(p => ({ userId: p.sub, userName: p.nickname })))
      .subscribe(user => {
        console.log('ActivityFeedComponent: got current user profile', user);
        this.currentUserInfo = user;

        // load user to get venue info
        this.load();
        this.loadDrinks();

        this.userService.getUser(user.userId).subscribe(
          u => {
            console.log('loaded user', u);
            this.currentUser = u;
            this.locationService.setCurrentVenue(u.currentVenue);
          },
          e => console.error('error loading user', user.userId, e)
        );
      })
    );

    this.subscriptions.push(
      this.locationService.locationChanged$.subscribe(() => {
        console.log('location has changed');

        this.snackBar
          .open(this.translateService.instant('NewVenue'), this.translateService.instant('YesCheckin'), { duration: 3000 })
          .onAction()
          .subscribe(() => {
            this.onCheckInVenue(VenueDialogMode.LocationChanged);
          });
      })
    );
  }

  loadDrinks() {
    this.drinkService.getDrinks().subscribe(
      d => {
        this.drinks = d;
        console.log('successfully got drinks', this.drinks);
      },
      e => {
        console.error('failed to get drinks', e);
      }
    );
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
    this.activityService.getActivityFeed(continuationToken).subscribe(
      d => {
        this.continuationToken = d.continuationToken;

        if (continuationToken) {
          this.activitys.push(...d.resultPage);
        } else {
          this.activitys = d.resultPage;
        }
        this.isBusy = false;

        this.isInitialLoad = false;

        // adds reload-spinner after a second so it doesn't interfere with slide-in animation
        setTimeout(() => (this.isReloadSpinnerActive = true), 3000);
      },
      e => {
        this.isBusy = false;
        console.error(e);
      }
    );
  }

  onDrink(drink: Drink) {
    this.isBusyAdding = true;

    this.drinkActivityService.drink(drink).subscribe(
      r => {
        this.isBusyAdding = false;
        this.load();
      },
      e => {
        this.isBusyAdding = false;
        this.shellInteraction.showErrorMessage();
      }
    );
  }

  onAddMessage() {
    this.dialog
      .open(MessageDialogComponent, { width: '80%' })
      .afterClosed()
      .pipe(filter(message => message))
      .subscribe(message => {
        this.isBusyAdding = true;
        const activity: AddMessageActivityDTO = {
          message: message,
          location: this.locationService.getCurrentLocation()
        };

        this.activityService.addMessageActivity(activity).subscribe(
          r => {
            this.isBusyAdding = false;
            this.load();
          },
          e => {
            this.isBusyAdding = false;
            this.shellInteraction.showErrorMessage();
          }
        );
      });
  }

  onCheckInVenue(mode: VenueDialogMode = VenueDialogMode.Default) {
    const args: VenueDialogArgs = {
      location: this.locationService.getCurrentLocation(),
      mode: mode
    };

    console.log('venueDialogArgs', args);

    this.dialog
      .open(VenueDialogComponent, { data: args, width: '90%' })
      .afterClosed()
      .subscribe((result: VenueDialogResult) => {
        switch (result.action) {
          case 'leave': {
            this.onResetVenue();
            break;
          }
          case 'change': {
            this.locationService.setCurrentVenue(result.venue).subscribe(_ => this.load());
            break;
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

    this.dialog
      .open(ConfirmationDialogComponent, { data: args })
      .afterClosed()
      .pipe(filter(isConfirmed => isConfirmed))
      .subscribe(_ => this.locationService.resetCurrentVenue().subscribe(() => this.load()));
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
