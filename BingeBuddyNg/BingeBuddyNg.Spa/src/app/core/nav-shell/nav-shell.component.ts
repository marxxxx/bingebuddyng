import { StateService } from '../state.service';
import { ShellIconInfo } from '../../../models/ShellIconInfo';
import { FriendRequestInfo } from '../../../models/FriendRequestInfo';
import { FriendRequestService } from '../friendrequest.service';
import { ShellInteractionService } from '../shell-interaction.service';
import { AuthService } from '../auth.service';
import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { BehaviorSubject, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';
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
    friendRequests: FriendRequestInfo[] = [];


    @ViewChild(MatSidenav)
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

        const loggedInSubScription = this.auth.currentUserProfile$.subscribe(profile => {
            if (profile != null) {
                this.updateUserAndFriendRequests(profile);
            }
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
