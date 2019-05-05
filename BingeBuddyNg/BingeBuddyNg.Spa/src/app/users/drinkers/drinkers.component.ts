import { Subscription, forkJoin } from 'rxjs';
import { FriendRequestInfo } from '../../../models/FriendRequestInfo';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { AuthService } from '../../core/auth.service';
import { UserInfo } from '../../../models/UserInfo';
import { User } from '../../../models/User';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../core/user.service';
import { FriendRequestService } from '../../core/friendrequest.service';
import { ActivatedRoute } from '@angular/router';
import { ShellInteractionService } from '../../core/shell-interaction.service';

@Component({
  selector: 'app-drinkers',
  templateUrl: './drinkers.component.html',
  styleUrls: ['./drinkers.component.css']
})
export class DrinkersComponent implements OnInit, OnDestroy {

  isBusy = false;
  filterText = null;
  currentUserId: string;
  currentUser: User;
  users: UserInfo[];
  pendingRequests: FriendRequestInfo[] = [];
  subs: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private auth: AuthService,
    private translate: TranslateService,
    private friendRequests: FriendRequestService,
    private shellInteractionService: ShellInteractionService,
    private snackbar: MatSnackBar) { }

  ngOnInit() {

    this.subs.push(this.auth.currentUserProfile$.subscribe(profile => {
      if (profile) {
        this.currentUserId = profile.sub;
      }
    }));

    this.subs.push(this.route.paramMap.subscribe(p => {
      this.load();
    }));
  }

  ngOnDestroy() {
    this.subs.forEach(s => s.unsubscribe());
  }


  load(): void {
    this.isBusy = true;

    forkJoin([
      this.userService.getUser(this.currentUserId),
      this.userService.getAllUsers(this.filterText),
      this.friendRequests.getPendingFriendRequests()
    ]
    ).subscribe(([user, allUsers, friendsRequests]) => {
      this.currentUser = user;
      this.users = allUsers;
      this.pendingRequests = friendsRequests;
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      this.shellInteractionService.showErrorMessage();
    });
  }

  onKeyDown(ev) {
    if (ev.key === 'Enter') {
      this.load();
    }
  }

  isYourFriend(u: UserInfo): boolean {
    return this.currentUser && this.currentUser.friends.findIndex(f => f.userId === u.userId) >= 0;
  }

  isYou(u: UserInfo): boolean {
    return this.currentUser && this.currentUser.id === u.userId;
  }

  hasPendingRequest(u: UserInfo): boolean {
    return this.pendingRequests.findIndex(r => r.requestingUser.userId === u.userId ||
      r.friendUser.userId === u.userId) >= 0;
  }
  onSendFriendRequest(u: UserInfo) {

    (<any>u).isBusy = true;

    this.friendRequests.addFriendRequest(u.userId).subscribe(r => {
      (<any>u).isBusy = false;
      const request: FriendRequestInfo = {
        friendUser: u,
        requestingUser:
        {
          userId: this.currentUser.id,
          userName: this.currentUser.name
        }
      };
      this.pendingRequests.push(request);
      const message = this.translate.instant('SentFriendRequest');
      this.snackbar.open(message, 'OK', { duration: 2000 });
    }, e => {
      (<any>u).isBusy = false;
      this.shellInteractionService.showErrorMessage();
    });
  }
}
