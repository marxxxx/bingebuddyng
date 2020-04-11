import { StateService } from '../../services/state.service';
import { ShellIconInfo } from '../../../../models/ShellIconInfo';
import { FriendRequestDTO } from '../../../../models/FriendRequestDTO';
import { FriendRequestService } from '../../services/friendrequest.service';
import { ShellInteractionService } from '../../services/shell-interaction.service';
import { AuthService } from '../../services/auth.service';
import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { BehaviorSubject, Subscription } from 'rxjs';
import { map, filter } from 'rxjs/operators';
import { MatSidenav } from '@angular/material/sidenav';
import { Router } from '@angular/router';
import { UserProfile } from 'src/models/UserProfile';

@Component({
  selector: 'app-nav-shell',
  templateUrl: './nav-shell.component.html',
  styleUrls: ['./nav-shell.component.scss']
})
export class NavShellComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription[] = [];
  private currentUserId: string;
  friendRequests: FriendRequestDTO[] = [];


  @ViewChild(MatSidenav, { static: true })
  sideNav: MatSidenav;

  isHandset$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  constructor(private breakpointObserver: BreakpointObserver,
    public auth: AuthService, public shellInteraction: ShellInteractionService,
    private friendRequestService: FriendRequestService,
    private router: Router,
    private state: StateService) {
    this.breakpointObserver.observe(Breakpoints.Handset)
      .pipe(
        map(result => result.matches)
      ).subscribe(this.isHandset$);
  }

  ngOnInit() {
    this.shellInteraction.registerSideNav(this.sideNav);

    const loggedInSubScription = this.auth.currentUserProfile$
      .pipe(filter(profile => profile != null))
      .subscribe(profile => {
        this.currentUserId = profile.toUserInfo().userId;
        this.updateUserAndFriendRequests(profile);
      });

    // update pending friend requests if something changed in this area
    const sub = this.state.pendingFriendRequestsChanged$.subscribe(p => this.updatePendingFriendRequests(this.currentUserId));

    this.subscriptions.push(sub, loggedInSubScription);
  }

  updateUserAndFriendRequests(profile: UserProfile) {
    this.updatePendingFriendRequests(profile.sub);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }


  private updatePendingFriendRequests(currentUserId: any) {
    this.friendRequestService.getPendingFriendRequests().subscribe(r => {
      this.friendRequests = r.filter(f => f.requestingUser.userId !== currentUserId);
    });
  }

  onLogout() {
    this.auth.logout();
  }

  onNavListItemClicked() {
    if (this.isHandset$.value) {
      this.sideNav.close();
    }
  }

  onShellIconClick(icon: ShellIconInfo): void {
    if (icon.link) {
      this.router.navigate([icon.link]);
    }
  }

}
