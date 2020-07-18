import { Component, ViewChild, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';

import { MatSidenav } from '@angular/material/sidenav';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { BehaviorSubject, Subscription } from 'rxjs';
import { map, filter } from 'rxjs/operators';

import { ShellInteractionService } from '../../services/shell-interaction.service';
import { AuthService } from '../../services/auth/auth.service';
import { UserDTO } from 'src/models/UserDTO';
import { ShellIconInfo } from '../../../../models/ShellIconInfo';

@Component({
  selector: 'app-nav-shell',
  templateUrl: './nav-shell.component.html',
  styleUrls: ['./nav-shell.component.scss']
})
export class NavShellComponent implements OnInit, OnDestroy {

  private subscriptions: Subscription[] = [];
  currentUser: UserDTO;

  @ViewChild(MatSidenav, { static: true })
  sideNav: MatSidenav;

  isHandset$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(true);

  constructor(
    public auth: AuthService,
    public shellInteraction: ShellInteractionService,
    private breakpointObserver: BreakpointObserver,
    private router: Router) {
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
        this.currentUser = profile.user;
      });


    this.subscriptions.push(loggedInSubScription);
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
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
