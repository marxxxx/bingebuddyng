import { ShellInteractionService } from '../../services/shell-interaction.service';
import { AuthService } from '../../services/auth.service';
import { Component, ViewChild } from '@angular/core';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';

@Component({
  selector: 'app-nav-shell',
  templateUrl: './nav-shell.component.html',
  styleUrls: ['./nav-shell.component.css']
})
export class NavShellComponent {

  @ViewChild(MatSidenav)
  sideNav: MatSidenav;

  isHandset$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);

  constructor(private breakpointObserver: BreakpointObserver,
    public auth: AuthService, public shellInteraction: ShellInteractionService) {
    this.breakpointObserver.observe(Breakpoints.Handset)
      .pipe(
        map(result => result.matches)
      ).subscribe(this.isHandset$);
  }


  onLogout() {
    this.auth.logout();
  }

  onNavListItemClicked() {
    if (this.isHandset$.value) {
      this.sideNav.close();
    }
  }

}
