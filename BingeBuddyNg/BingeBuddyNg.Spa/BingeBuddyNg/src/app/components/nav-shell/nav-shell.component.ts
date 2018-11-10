import { StateService } from './../../services/state.service';
import { ShellIconInfo } from './../../../models/ShellIconInfo';
import { FriendRequestInfo } from './../../../models/FriendRequestInfo';
import { FriendRequestService } from './../../services/friendrequest.service';
import { ShellInteractionService } from '../../services/shell-interaction.service';
import { AuthService } from '../../services/auth.service';
import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { Observable, Subject, BehaviorSubject, Subscription } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';
import { Router } from '@angular/router';

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

    isHandset$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

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

        const loggedInSubScription = this.auth.isLoggedIn$.subscribe(isLoggedIn => {
            if (isLoggedIn) {
                this.updateUserAndFriendRequests();
            }
        });

        if (this.auth.isLoggedIn$.value) {
            this.updateUserAndFriendRequests();
        }

        // update pending friend requests if something changed in this area
        const sub = this.state.pendingFriendRequestsChanged$.subscribe(p => this.updatePendingFriendRequests(this.currentUserId));

        this.subscriptions.push(sub, loggedInSubScription);
    }

    updateUserAndFriendRequests() {
        // get current user
        this.auth.getProfile((err, profile) => {
            this.currentUserId = profile.sub;

            // get pending friend requests
            this.updatePendingFriendRequests(this.currentUserId);
        });
    }

    ngOnDestroy() {
        this.subscriptions.forEach(s => s.unsubscribe());
    }


    private updatePendingFriendRequests(currentUserId: any) {
        this.friendRequestService.getPendingFriendRequests().subscribe(r => {
            this.friendRequests = r.filter(f => f.requestingUser.userId !== currentUserId);
            const id = 'friendrequests';
            if (this.friendRequests.length > 0) {
                this.shellInteraction.addShellIcon({
                    id: id,
                    name: 'sentiment_satisfied_alt',
                    tooltip: 'PendingFriendRequests',
                    link: '/friendrequests'
                });
            } else {
                this.shellInteraction.removeShellIcon(id);
            }
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
