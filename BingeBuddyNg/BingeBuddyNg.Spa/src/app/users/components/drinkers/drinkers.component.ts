import { Subscription, forkJoin, combineLatest } from 'rxjs';
import { FriendRequestDTO } from '../../../../models/FriendRequestDTO';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../@core/services/auth.service';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { UserDTO } from '../../../../models/UserDTO';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { UserService } from '../../../@core/services/user.service';
import { FriendRequestService } from '../../../@core/services/friendrequest.service';
import { ActivatedRoute } from '@angular/router';
import { ShellInteractionService } from '../../../@core/services/shell-interaction.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-drinkers',
  templateUrl: './drinkers.component.html',
  styleUrls: ['./drinkers.component.css']
})
export class DrinkersComponent implements OnInit, OnDestroy {

  isBusy = false;
  filterText = null;
  currentUserId: string;
  currentUser: UserDTO;
  users: UserInfoDTO[];
  pendingRequests: FriendRequestDTO[] = [];
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

    this.subs.push(
      combineLatest([
        this.auth.currentUserProfile$.pipe(filter(profile => profile != null)),
        this.route.paramMap]
      )
      .subscribe(r => {
        this.currentUserId = r[0].sub;
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

  isYourFriend(u: UserInfoDTO): boolean {
    return this.currentUser && this.currentUser.friends.findIndex(f => f.userId === u.userId) >= 0;
  }

  isYou(u: UserInfoDTO): boolean {
    return this.currentUser && this.currentUser.id === u.userId;
  }

  hasPendingRequest(u: UserInfoDTO): boolean {
    return this.pendingRequests.findIndex(r => r.requestingUser.userId === u.userId ||
      r.friendUser.userId === u.userId) >= 0;
  }
  onSendFriendRequest(u: UserInfoDTO) {

    (<any>u).isBusy = true;

    this.friendRequests.addFriendRequest(u.userId).subscribe(r => {
      (<any>u).isBusy = false;
      const request: FriendRequestDTO = {
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
