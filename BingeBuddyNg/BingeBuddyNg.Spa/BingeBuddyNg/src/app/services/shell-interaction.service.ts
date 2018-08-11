import { Injectable } from '@angular/core';
import { ShellIconInfo } from '../../models/ShellIconInfo';

@Injectable({
  providedIn: 'root'
})
export class ShellInteractionService {

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


}
