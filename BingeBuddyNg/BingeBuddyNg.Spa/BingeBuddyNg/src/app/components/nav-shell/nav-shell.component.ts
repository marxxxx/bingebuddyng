import { ShellInteractionService } from '../../services/shell-interaction.service';
import { AuthService } from '../../services/auth.service';
import { Component, ViewChild, OnInit } from '@angular/core';
import { BreakpointObserver, Breakpoints, BreakpointState } from '@angular/cdk/layout';
import { Observable, Subject, BehaviorSubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatSidenav } from '@angular/material';

@Component({
  selector: 'app-nav-shell',
  templateUrl: './nav-shell.component.html',
  styleUrls: ['./nav-shell.component.scss']
})
export class NavShellComponent implements OnInit {

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

  ngOnInit() {
    this.shellInteraction.registerSideNav(this.sideNav);


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
