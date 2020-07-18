import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription, forkJoin, combineLatest } from 'rxjs';
import { filter } from 'rxjs/operators';
import { TranslocoService } from '@ngneat/transloco';

import { UserService } from '../../../@core/services/user.service';
import { FriendRequestDTO } from '../../../../models/FriendRequestDTO';
import { AuthService } from '../../../@core/services/auth/auth.service';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { UserDTO } from '../../../../models/UserDTO';
import { ShellInteractionService } from '../../../@core/services/shell-interaction.service';
import { FriendRequestDirection } from 'src/models/FriendRequestDirection';

@Component({
  selector: 'app-drinkers',
  templateUrl: './drinkers.component.html',
  styleUrls: ['./drinkers.component.css'],
})
export class DrinkersComponent implements OnInit, OnDestroy {
  isBusy = false;
  filterText = null;
  currentUserId: string;
  currentUser: UserDTO;
  users: UserInfoDTO[];
  subs: Subscription[] = [];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private auth: AuthService,
    private translate: TranslocoService,
    private shellInteractionService: ShellInteractionService,
    private snackbar: MatSnackBar
  ) {}

  ngOnInit() {
    this.subs.push(
      combineLatest([this.auth.currentUserProfile$.pipe(filter((profile) => profile != null)), this.route.paramMap]).subscribe((r) => {
        this.currentUserId = r[0].sub;
        this.load();
      })
    );
  }

  ngOnDestroy() {
    this.subs.forEach((s) => s.unsubscribe());
  }

  load(): void {
    this.isBusy = true;

    forkJoin([this.userService.getUser(this.currentUserId), this.userService.getAllUsers(this.filterText)]).subscribe(
      ([user, allUsers]) => {
        this.currentUser = user;
        this.users = allUsers;
        this.isBusy = false;
      },
      (e) => {
        this.isBusy = false;
        this.shellInteractionService.showErrorMessage();
      }
    );
  }

  onKeyDown(ev) {
    if (ev.key === 'Enter') {
      this.load();
    }
  }

  isYourFriend(u: UserInfoDTO): boolean {
    return this.currentUser && this.currentUser.friends.findIndex((f) => f.userId === u.userId) >= 0;
  }

  isYou(u: UserInfoDTO): boolean {
    return this.currentUser && this.currentUser.id === u.userId;
  }

  hasPendingRequest(u: UserInfoDTO): boolean {
    if (!this.currentUser) {
      return false;
    }
    return (
      this.currentUser.outgoingFriendRequests?.some((r) => r.user.userId === u.userId) ||
      this.currentUser.incomingFriendRequests?.some((r) => r.user.userId === u.userId)
    );
  }
  onSendFriendRequest(u: UserInfoDTO) {
    (<any>u).isBusy = true;

    this.userService.addFriendRequest(u.userId).subscribe(
      (r) => {
        (<any>u).isBusy = false;

        const request: FriendRequestDTO = {
          user: u,
          direction: FriendRequestDirection.Outgoing,
          requestTimestamp: new Date().toISOString(),
        };

        if (!this.currentUser.outgoingFriendRequests) {
          this.currentUser.outgoingFriendRequests = [];
        }
        this.currentUser.outgoingFriendRequests.push(request);

        const message = this.translate.translate('SentFriendRequest');
        this.snackbar.open(message, 'OK', { duration: 2000 });
      },
      (e) => {
        (<any>u).isBusy = false;
        this.shellInteractionService.showErrorMessage();
      }
    );
  }
}
