import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { filter, map } from 'rxjs/operators';

import { StateService } from '../../../@core/services/state.service';
import { AuthService } from '../../../@core/services/auth/auth.service';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { UserService } from 'src/app/@core/services/user.service';
import { UserDTO } from 'src/models/UserDTO';

@Component({
  selector: 'app-friendrequests',
  templateUrl: './friendrequests.component.html',
  styleUrls: ['./friendrequests.component.css']
})
export class FriendrequestsComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription[] = [];
  private currentUser: UserDTO;
  isBusy = false;

  constructor(
    private userService: UserService,
    private state: StateService,
    private auth: AuthService,
    private route: ActivatedRoute) { }

  ngOnInit() {

    // get current user
    const sub = combineLatest([
      this.auth.currentUserProfile$.pipe(filter(p => p != null)).pipe(map(p => p.user)),
      this.route.paramMap
    ]).subscribe(([user, _]) => {
      this.currentUser = user;
    });

    this.subscriptions.push(sub);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }

  onAccept(user: UserInfoDTO) {

    this.removeUserFromList(user.userId);
    this.userService.acceptFriendRequest(user.userId).subscribe(r => {

      // signal change in friend requests status
      this.state.raisePendingFriendRequestsChanged();
    });

  }

  onDecline(user: UserInfoDTO) {
    this.removeUserFromList(user.userId);
    this.userService.declineFriendRequest(user.userId).subscribe(r => {

      // signal change in friend requests status
      this.state.raisePendingFriendRequestsChanged();
    });
  }

  removeUserFromList(userId: string) {
    const index = this.currentUser.incomingFriendRequests.findIndex(r => r.user.userId === userId);
    this.currentUser.incomingFriendRequests.splice(index, 1);
  }
}
