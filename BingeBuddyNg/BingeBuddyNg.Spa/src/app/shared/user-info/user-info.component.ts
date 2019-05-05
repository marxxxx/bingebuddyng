import { Router } from '@angular/router';
import { UserService } from '../../core/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { UserInfo } from '../../../models/UserInfo';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {

  @Input()
  userInfo: UserInfo;

  @Input()
  showName: boolean;

  constructor(private userService: UserService) { }

  ngOnInit() {
  }

  getProfileImageUrl() {
    if (this.userInfo == null) {
      return null;
    }
    return this.userService.getProfileImageUrl(this.userInfo.userId);
  }
}
