import { DrinkActivityService } from '../../services/drink-activity.service';
import { ConfirmationDialogComponent } from '../../../@shared/components/confirmation-dialog/confirmation-dialog.component';
import { ConfirmationDialogArgs } from '../../../@shared/components/confirmation-dialog/ConfirmationDialogArgs';
import { UserService } from 'src/app/@core/services/user.service';
import { VenueDialogArgs } from '../venue-dialog/VenueDialogArgs';
import { VenueDialogComponent } from '../venue-dialog/venue-dialog.component';
import { MessageDialogComponent } from '../message-dialog/message-dialog.component';
import { DrinkType } from '../../../../models/DrinkType';
import { Subscription } from 'rxjs';
import { map, filter } from 'rxjs/operators';
import { ActivityStatsDTO } from '../../../../models/ActivityStatsDTO';
import { ActivityService } from '../../services/activity.service';
import { Component, OnInit, OnDestroy, ViewChild, ChangeDetectorRef, ViewChildren, ElementRef } from '@angular/core';
import { AuthService } from '../../../@core/services/auth.service';
import { AddMessageActivityDTO } from '../../../../models/AddMessageActivityDTO';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatTooltip } from '@angular/material/tooltip';
import { ShellInteractionService } from '../../../@core/services/shell-interaction.service';
import { FileUploader, FileItem, FileUploaderOptions } from 'ng2-file-upload';
import { NotificationService } from '../../../@core/services/notification.service';
import { trigger, style, transition, animate } from '@angular/animations';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { UserDTO } from '../../../../models/UserDTO';
import { LocationService } from 'src/app/activity/services/location.service';
import { VenueDialogMode } from '../venue-dialog/VenueDialogMode';
import { VenueDialogResult } from '../venue-dialog/VenueDialogResult';
import { TranslocoService } from '@ngneat/transloco';
import { Drink } from 'src/models/Drink';
import { DrinkRetrieverService } from '../../services/drink-retriever.service';
import { ActivatedRoute } from '@angular/router';

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
  locationIconId = 'location';
  uploader: FileUploader;
  continuationToken: string = null;
  isInitialLoad = true;
  isBusyUploading = false;
  currentProgress = 0;
  isReloadSpinnerActive = false;
  currentUserInfo: UserInfoDTO;
  currentUser: UserDTO;
  DrinkType = DrinkType;
  isCommentOpen = false;
  drinks: Drink[];
  highlightedActivityId: string;
  pendingDrinkType: DrinkType;

  @ViewChild('#activity-container', { static: false })
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
    private translateService: TranslocoService,
    private dialog: MatDialog,
    private route: ActivatedRoute
  ) { }

  ngOnInit() {
    this.initFileUploader();

    this.isBusy = true;
    this.route.queryParamMap.subscribe(p => {
      this.highlightedActivityId = p.get('activityId');
      this.load();
    });

    this.subscriptions.push(this.notification.activityReceived$.subscribe(_ => this.load()));
    this.subscriptions.push(
      this.auth.currentUserProfile$
        .pipe(filter(p => p != null))
        .pipe(map(p => ({ userId: p.sub, userName: p.nickname })))
        .subscribe(user => {
          console.log('ActivityFeedComponent: got current user profile', user);
          this.currentUserInfo = user;

          this.loadDrinks();

          // load user to get venue info
          this.userService.getUser(user.userId).subscribe(
            u => {
              this.currentUser = u;
              this.locationService.setCurrentVenue(u.currentVenue);
            },
            e => console.error('error loading user', user.userId, e)
          );
        })
    );

    this.subscriptions.push(
      this.locationService.locationChanged$.subscribe(() => {
        this.snackBar
          .open(this.translateService.translate('NewVenue'), this.translateService.translate('YesCheckin'), { duration: 3000 })
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
    this.isBusy = true;
    this.activityService.getActivityFeed(null, continuationToken).subscribe(
      d => {
        this.continuationToken = d.continuationToken;

        if (continuationToken) {
          this.activitys.push(...d.resultPage);
        } else {
          this.activitys = d.resultPage;
        }
        this.isBusy = false;
        this.isInitialLoad = false;

        if (this.highlightedActivityId) {

          setTimeout(() => {
            const activityElement = document.getElementById(this.highlightedActivityId);
            if (activityElement) {
              activityElement.scrollIntoView();
            }
          }, 1000);
        }

        // adds reload-spinner after a second so it doesn't interfere with slide-in animation
        setTimeout(() => (this.isReloadSpinnerActive = true), 3000);
      },
      e => {
        this.isBusy = false;
        this.isInitialLoad = false;
        console.error(e);
      }
    );
  }

  onDrink(drink: Drink) {
    this.pendingDrinkType = drink.drinkType;
    this.isBusyAdding = true;

    this.drinkActivityService.drink(drink, false)
      .subscribe(
        r => {
          if (this.isBusyAdding) {
            this.isBusyAdding = false;
            this.load();
          }
        },
        e => {
          this.isBusyAdding = false;
          this.shellInteraction.showErrorMessage();
        }
      );
  }

  onCancelDrinkAnimation() {
    this.isBusyAdding = false;
    this.load();
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
    if (open) {
      setTimeout(() => this.tooltips.forEach(t => t.show()), 500);
    } else {
      this.tooltips.forEach(t => t.hide());
    }
  }

  trackByActivity(index, item: ActivityStatsDTO) {
    return item.activity.id;
  }
}
