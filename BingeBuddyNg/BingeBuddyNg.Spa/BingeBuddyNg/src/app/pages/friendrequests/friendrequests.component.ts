import { StateService } from './../../services/state.service';
import { AuthService } from './../../services/auth.service';
import { UserInfo } from './../../../models/UserInfo';
import { FriendRequestService } from './../../services/friendrequest.service';
import { ActivatedRoute } from '@angular/router';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-friendrequests',
  templateUrl: './friendrequests.component.html',
  styleUrls: ['./friendrequests.component.css']
})
export class FriendrequestsComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription[] = [];
  private currentUserId: string;
  pendingRequests: UserInfo[] = [];
  isBusy = false;

  constructor(private route: ActivatedRoute, private friendRequest: FriendRequestService,
    private state: StateService,
    private auth: AuthService) { }

  ngOnInit() {

    // get current user id
    this.auth.getProfile((err, profile) => this.currentUserId = profile.sub);

    // load friend requests
    const sub = this.route.paramMap.subscribe(r => {
      this.load();
    });

    this.subscriptions.push(sub);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }


  load(): void {
    this.isBusy = true;
    this.friendRequest.getPendingFriendRequests().subscribe(r => {
      this.pendingRequests = r.filter(f => f.requestingUser.userId !== this.currentUserId)
        .map(f => f.requestingUser);
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  onAccept(user: UserInfo) {

    this.removeUserFromList(user.userId);
    this.friendRequest.acceptFriendRequest(user.userId).subscribe(r => {

      // signal change in friend requests status
      this.state.raisePendingFriendRequestsChanged();
    });

  }

  onDecline(user: UserInfo) {
    this.removeUserFromList(user.userId);
    this.friendRequest.declineFriendRequest(user.userId).subscribe(r => {

      // signal change in friend requests status
      this.state.raisePendingFriendRequestsChanged();
    });
  }

  removeUserFromList(userId: string) {
    const index = this.pendingRequests.findIndex(r => r.userId === userId);
    this.pendingRequests.splice(index, 1);
  }
}
