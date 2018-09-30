import { MatSidenav } from '@angular/material';
import { Injectable } from '@angular/core';
import { ShellIconInfo } from '../../models/ShellIconInfo';

@Injectable({
  providedIn: 'root'
})
export class ShellInteractionService {

  private sideNav: MatSidenav;

  shellIcons: ShellIconInfo[] = [];

  constructor() { }

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

}
