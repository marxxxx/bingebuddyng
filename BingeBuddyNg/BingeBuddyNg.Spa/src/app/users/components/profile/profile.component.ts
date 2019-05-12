import { ShellInteractionService } from '../../../core/services/shell-interaction.service';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { FriendRequestService } from '../../../core/services/friendrequest.service';
import { Subscription, combineLatest } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../../core/services/user.service';
import { AuthService } from '../../../core/services/auth.service';
import { User } from '../../../../models/User';
import { filter } from 'rxjs/internal/operators/filter';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {
  private subscriptions: Subscription[] = [];

  user: User;
  originalUserName: string;
  currentUserId: string;
  userId: string;
  hasPendingRequest = false;
  isBusy = false;
  isEditingUserName = false;
  isBusyUpdatingUserName = false;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private friendRequests: FriendRequestService,
    private auth: AuthService,
    private translate: TranslateService,
    private snackBar: MatSnackBar,
    private shellInteraction: ShellInteractionService
  ) {}

  ngOnInit() {
    console.log('on init called');
    const sub = combineLatest([this.route.paramMap, this.auth.currentUserProfile$.pipe(filter(p => p != null))]).subscribe(result => {
      console.log('got information for profile page', result);
      this.userId = decodeURIComponent(result[0].get('userId'));
      this.currentUserId = this.auth.currentUserProfile$.value.sub;

      this.load();
    });

    this.subscriptions.push(sub);
  }

  load(): void {
    console.log('loading user profile', this.userId);
    this.isBusy = true;

    this.userService.getUser(this.userId).subscribe(
      r => {
        console.log('got user information', r);

        this.user = r;
        this.originalUserName = r.name;

        if (!this.isYou()) {
          this.friendRequests.hasPendingFriendRequests(this.userId).subscribe(
            hasRequest => {
              this.hasPendingRequest = hasRequest;
              this.isBusy = false;
            },
            e => {
              this.isBusy = false;
              console.error('error retrieving pending friend request status');
              console.error(e);
            }
          );
        } else {
          this.isBusy = false;
          // TODO: clean up this messy code!
        }
      },
      e => {
        console.error(e);
        this.isBusy = false;
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
    this.friendRequests.addFriendRequest(this.userId).subscribe(
      r => {
        const message = this.translate.instant('SentFriendRequest');
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
        const message = this.translate.instant('RemovedFriend');

        this.snackBar.open(message, 'OK', { duration: 2000 });
      },
      e => {
        this.shellInteraction.showErrorMessage();
      }
    );
  }

  isMutedFriend(userId: string): boolean {
    return this.user && this.user.mutedFriendUserIds && this.user.mutedFriendUserIds.indexOf(userId) >= 0;
  }

  setFriendMuteState(userId: string, mute: boolean) {
    if (this.user.mutedFriendUserIds == null) {
      this.user.mutedFriendUserIds = [];
    }

    console.log(`setting friend mute state ${userId} - ${mute}`);
    if (mute) {
      this.user.mutedFriendUserIds.push(userId);
    } else {
      const index = this.user.mutedFriendUserIds.findIndex(u => u === userId);
      if (index >= 0) {
        this.user.mutedFriendUserIds.splice(index, 1);
      }
    }

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
    this.userService.saveUser(this.user).subscribe( () => {
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
}
