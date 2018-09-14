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
  pendingRequests: UserInfo[] = [];
  isBusy = false;

  constructor(private route: ActivatedRoute, private friendRequest: FriendRequestService) { }

  ngOnInit() {

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
      this.pendingRequests = r;
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  onAccept(user: UserInfo) {

    this.removeUserFromList(user.userId);
    this.friendRequest.acceptFriendRequest(user.userId).subscribe(r => {
      console.log('accepted');
    });

  }

  onDecline(user: UserInfo) {
    this.removeUserFromList(user.userId);
    this.friendRequest.declineFriendRequest(user.userId).subscribe(r => {
      console.log('accepted');
    });
  }

  removeUserFromList(userId: string) {
    const index = this.pendingRequests.findIndex(r => r.userId === userId);
    this.pendingRequests.splice(index, 1);
  }
}
