import { FriendRequestInfo } from './../../../models/FriendRequestInfo';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { AuthService } from './../../services/auth.service';
import { UserInfo } from './../../../models/UserInfo';
import { User } from './../../../models/User';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';
import { FriendRequestService } from '../../services/friendrequest.service';

@Component({
  selector: 'app-drinkers',
  templateUrl: './drinkers.component.html',
  styleUrls: ['./drinkers.component.css']
})
export class DrinkersComponent implements OnInit {

  isBusy = false;
  filterText = null;
  currentUser: User;
  users: UserInfo[];
  pendingRequests: FriendRequestInfo[] = [];

  constructor(private userService: UserService,
    private auth: AuthService,
    private translate: TranslateService,
    private friendRequests: FriendRequestService,
    private snackbar: MatSnackBar) { }

  ngOnInit() {
    // get current user
    this.auth.getProfile((err, profile) => {

      this.userService.getUser(profile.sub).subscribe(u => {
        this.currentUser = u;
      });
    });

    this.friendRequests.getPendingFriendRequests().subscribe(r => {
      this.pendingRequests = r;
    });
  }


  load(): void {
    this.isBusy = true;

    this.userService.getAllUsers(this.filterText).subscribe(r => {
      this.users = r;
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      console.error(e);
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
          userName: this.currentUser.name,
          userProfileImageUrl: this.currentUser.profileImageUrl
        }
      };
      this.pendingRequests.push(request);
      const message = this.translate.instant('SentFriendRequest');
      this.snackbar.open(message, 'OK', { duration: 2000 });
    }, e => {
      (<any>u).isBusy = false;
    });
  }
}
