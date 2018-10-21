import { MatSidenav, MatSnackBar } from '@angular/material';
import { Injectable } from '@angular/core';
import { ShellIconInfo } from '../../models/ShellIconInfo';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class ShellInteractionService {

  private sideNav: MatSidenav;

  shellIcons: ShellIconInfo[] = [];

  constructor(private translate: TranslateService, private snackbar: MatSnackBar) { }

  addShellIcon(info: ShellIconInfo) {
    const existingIconIndex = this.shellIcons.findIndex(i => i.id === info.id);
    if (existingIconIndex >= 0) {
      this.shellIcons[existingIconIndex] = info;
    } else {
      this.shellIcons.push(info);
    }
  }

  removeShellIcon(id: string) {
    const existingIconIndex = this.shellIcons.findIndex(i => i.id === id);
    if (existingIconIndex >= 0) {
      this.shellIcons.splice(existingIconIndex, 1);
    }
  }

  registerSideNav(sideNav: MatSidenav) {
    this.sideNav = sideNav;
  }

  setSideNavState(open: boolean) {
    if (open) {
      this.sideNav.open();
    } else {
      this.sideNav.close();
    }
  }

  showErrorMessage() {
    const message = this.translate.instant('OperationFailed');
    this.snackbar.open(message, 'OK');
  }
}
