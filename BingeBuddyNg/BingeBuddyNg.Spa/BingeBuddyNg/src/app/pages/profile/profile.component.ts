import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';
import { FriendRequestService } from './../../services/friendrequest.service';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { User } from '../../../models/User';
import { UserInfo } from '../../../models/UserInfo';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css']
})
export class ProfileComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription[] = [];

  user: User;
  currentUserId: string;
  userId: string;
  hasPendingRequest = false;
  isBusy = false;


  constructor(private route: ActivatedRoute,
    private userService: UserService,
    private friendRequests: FriendRequestService,
    private auth: AuthService,
    private translate: TranslateService,
    private snackBar: MatSnackBar) { }

  ngOnInit() {

    const sub = this.route.paramMap.subscribe(p => {
      this.userId = p.get('userId');

      this.load();

    });
    this.subscriptions.push(sub);
  }

  load(): void {

    this.isBusy = true;
    this.userService.getUser(this.userId).subscribe(r => {
      this.user = r;
      this.isBusy = false;
    }, e => {
      console.log(e);
    });

    this.auth.getProfile((err, profile) => {
      if (profile) {
        this.currentUserId = profile.sub;

        if (!this.isYou()) {
          this.friendRequests.hasPendingFriendRequests(this.userId).subscribe(r => this.hasPendingRequest = r);
        }
      } else {
        console.error('could not get profile');
      }
    });


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
    const isYourFriend = (this.user && this.user.friends && this.user.friends.find(f => f.userId === this.currentUserId) != null);
    return isYourFriend;
  }

  onAddFriend() {
    this.hasPendingRequest = true;
    this.friendRequests.addFriendRequest(this.userId).subscribe(r => {
      const message = this.translate.instant('SentFriendRequest');
      this.snackBar.open(message, 'OK', {duration: 2000});
    });
  }
}
