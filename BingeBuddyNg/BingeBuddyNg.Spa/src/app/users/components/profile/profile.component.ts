import { Component, OnInit, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { FileUploader, FileUploaderOptions, FileItem } from 'ng2-file-upload';
import { Subscription, combineLatest } from 'rxjs';
import { filter } from 'rxjs/internal/operators/filter';
import { TranslocoService } from '@ngneat/transloco';

import { CreateOrUpdateUserDTO } from 'src/models/CreateOrUpdateUserDTO';
import { ProfileImageDialogComponent } from './../profile-image-dialog/profile-image-dialog.component';
import { ShellInteractionService } from '../../../@core/services/shell-interaction.service';

import { UserService } from '../../../@core/services/user.service';
import { AuthService } from '../../../@core/services/auth/auth.service';
import { UserDTO } from '../../../../models/UserDTO';
import { ProfileImageDialogArgs } from '../profile-image-dialog/ProfileImageDialogArgs';
import { ConfirmationDialogArgs } from 'src/app/@shared/components/confirmation-dialog/ConfirmationDialogArgs';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  user: UserDTO;
  originalUserName: string;
  currentUserId: string;
  currentUser: UserDTO;
  userId: string;
  hasPendingRequest = false;
  isBusy = false;
  isEditingUserName = false;
  isBusyUpdatingUserName = false;
  isBusyUpdatingProfilePic = false;
  uploader: FileUploader;

  @ViewChild('fileUpload', { static: false })
  fileUpload: ElementRef;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private auth: AuthService,
    private translate: TranslocoService,
    private snackBar: MatSnackBar,
    private shellInteraction: ShellInteractionService,
    private dialog: MatDialog
  ) { }

  ngOnInit() {
    this.initFileUploader();
    const sub = combineLatest([this.route.paramMap, this.auth.currentUserProfile$.pipe(filter(p => p != null))])
      .subscribe(([paramMap, profile]) => {
        this.userId = decodeURIComponent(paramMap.get('userId'));
        this.currentUserId = profile.sub;
        this.currentUser = profile.user;

        this.load();
      });

    this.subscriptions.push(sub);
  }

  initFileUploader(): void {
    this.uploader = new FileUploader(this.getOptions());

    this.uploader.onAfterAddingFile = this.onAfterAddingFile.bind(this);
    this.uploader.onCompleteAll = this.onCompleteAll.bind(this);
  }

  onAfterAddingFile(fileItem: FileItem) {
    this.uploader.setOptions(this.getOptions());

    // upload
    this.isBusyUpdatingProfilePic = true;
    this.uploader.uploadAll();
  }

  getOptions(): FileUploaderOptions {
    const options = {
      url: this.userService.getUpdateProfileImageUrl(),
      authTokenHeader: 'Authorization',
      authToken: 'Bearer ' + this.auth.getAccessToken()
    };

    return options;
  }

  onCompleteAll() {
    this.uploader.clearQueue();
    this.isBusyUpdatingProfilePic = false;
    location.reload();
  }


  load(): void {
    console.log('loading user profile', this.userId);
    this.isBusy = true;

    this.userService.getUser(this.userId)
    .pipe(finalize(() => this.isBusy = false))
    .subscribe(
      r => {
        console.log('got user information', r);

        this.user = r;
        this.originalUserName = r.name;

        if (!this.isYou()) {
          this.hasPendingRequest = this.currentUser.outgoingFriendRequests?.some(i=>i.user.userId == this.userId);
        }

      },
      e => {
        console.error(e);
        this.shellInteraction.showErrorMessage();
      }
    );
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  isYou(): boolean {
    if (!this.user) {
      return false;
    }
    return this.user.id === this.currentUserId;
  }

  isYourFriend(): boolean {
    const isYourFriend = this.user && this.user.friends && this.user.friends.find(f => f.userId === this.currentUserId) != null;
    return isYourFriend;
  }

  onAddFriend() {
    this.hasPendingRequest = true;
    this.userService.addFriendRequest(this.userId).subscribe(
      r => {
        const message = this.translate.translate('SentFriendRequest');
        this.snackBar.open(message, 'OK', { duration: 2000 });
      },
      e => {
        this.shellInteraction.showErrorMessage();
      }
    );
  }

  onRemoveFriend() {
    const index = this.user.friends.findIndex(f => f.userId === this.currentUserId);
    if (index >= 0) {
      this.user.friends.splice(index, 1);
    }

    this.userService.removeFriend(this.userId).subscribe(
      r => {
        const message = this.translate.translate('RemovedFriend');

        this.snackBar.open(message, 'OK', { duration: 2000 });
      },
      e => {
        this.shellInteraction.showErrorMessage();
      }
    );
  }

  isMutedFriend(userId: string): boolean {
    return this.currentUser?.friends.some(f=>f.userId === userId && f.muted);
  }

  setFriendMuteState(userId: string, mute: boolean) {

    this.currentUser.friends.find(f=>f.userId === userId).muted = mute;

    this.userService.setFriendMuteState(userId, mute).subscribe(
      r => console.log('finished'),
      e => {
        this.shellInteraction.showErrorMessage();
      }
    );
  }

  getProfileImageUrl() {
    return this.userService.getProfileImageUrl(this.userId);
  }

  onUpdateUserName() {
    this.isBusyUpdatingUserName = true;
    const request = new CreateOrUpdateUserDTO(
      this.user.id,
      this.user.name,
      this.user.profileImageUrl,
      this.user.pushInfo,
      this.user.language);

    this.userService.createOrUpdateUser(request).subscribe(() => {
      this.originalUserName = this.user.name;
      this.isEditingUserName = false;
      this.isBusyUpdatingUserName = false;
      this.shellInteraction.showMessage('DataUpdateSuccessful');
    }, e => {
      console.error('error updating username', e);
      this.shellInteraction.showErrorMessage();
      this.isBusyUpdatingUserName = false;
    });
  }

  onProfileImageClick() {
    if (this.isYou()) {
      this.fileUpload.nativeElement.click();
    } else {
      const userProfileImageUrl = this.userService.getProfileImageUrl(this.userId);
      const args: ProfileImageDialogArgs = new ProfileImageDialogArgs(userProfileImageUrl);
      this.dialog.open(ProfileImageDialogComponent, {
        data: args,
        height: '90%'
      });
    }
  }

  onDeleteMyself() {
    const args: ConfirmationDialogArgs = {
      icon: 'delete',
      title: 'UserProfile.DeleteProfile',
      message: 'UserProfile.ReallyDeleteProfile',
      cancelButtonCaption: 'Cancel',
      confirmButtonCaption: 'Delete'
    };
    this.shellInteraction.showConfirmationDialog(args)
      .pipe(filter(isConfirmed => isConfirmed))
      .subscribe(() => {
        this.userService.deleteMyself().subscribe(() => {
          this.shellInteraction.showMessage('UserProfile.ProfileDeletionRequested')
            .subscribe(() => {
              this.auth.logout();
            });
        }, e => {
          this.shellInteraction.showErrorMessage();
        });
      });
  }
}
