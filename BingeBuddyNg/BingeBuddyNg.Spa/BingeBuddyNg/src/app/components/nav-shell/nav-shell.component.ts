import { ShellIconInfo } from './../../../models/ShellIconInfo';
import { FriendRequestInfo } from './../../../models/FriendRequestInfo';
import { FriendRequestService } from './../../services/friendrequest.service';
import { ShellInteractionService } from '../../services/shell-interaction.service';
import { AuthService } from '../../services/auth.service';
import { Component, ViewChild, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';
import { Router } from '@angular/router';

@Component({
    selector: 'app-nav-shell',
    templateUrl: './nav-shell.component.html',
    styleUrls: ['./nav-shell.component.scss']
})
export class NavShellComponent implements OnInit {

    friendRequests: FriendRequestInfo[] = [];

    @ViewChild(MatSidenav)
    sideNav: MatSidenav;

    isHandset$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

    constructor(private breakpointObserver: BreakpointObserver,
        public auth: AuthService, public shellInteraction: ShellInteractionService,
        private friendRequestService: FriendRequestService,
        private router: Router) {
        this.breakpointObserver.observe(Breakpoints.Handset)
            .pipe(
                map(result => result.matches)
            ).subscribe(this.isHandset$);
    }

    ngOnInit() {
        this.shellInteraction.registerSideNav(this.sideNav);

        // get current user
        this.auth.getProfile((err, profile) => {
            const currentUserId = profile.sub;

            // get pending friend requests
            this.friendRequestService.getPendingFriendRequests().subscribe(r => {
                this.friendRequests = r.filter(f => f.requestingUser.userId !== currentUserId);

                if (this.friendRequests.length > 0) {
                    this.shellInteraction.addShellIcon({
                        id: 'friendrequests',
                        name: 'sentiment_satisfied_alt',
                        tooltip: 'PendingFriendRequests',
                        link: '/friendrequests'
                    });
                }
            });
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
