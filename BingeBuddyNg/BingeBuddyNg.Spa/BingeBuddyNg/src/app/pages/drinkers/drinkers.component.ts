import { UserInfo } from './../../../models/UserInfo';
import { User } from './../../../models/User';
import { Component, OnInit } from '@angular/core';
import { UserService } from '../../services/user.service';

@Component({
  selector: 'app-drinkers',
  templateUrl: './drinkers.component.html',
  styleUrls: ['./drinkers.component.css']
})
export class DrinkersComponent implements OnInit {

  isBusy = false;
  filterText = null;
  users: UserInfo[];

  constructor(private userService: UserService) { }

  ngOnInit() {
  }

  load(): void {
    this.isBusy = true;

    this.userService.getAllUsers(this.filterText).subscribe(r => {
      this.users = r;
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  onKeyDown(ev) {
    if (ev.key === 'Enter') {
      this.load();
    }
  }
}
