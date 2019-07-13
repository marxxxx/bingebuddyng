import { Router } from '@angular/router';
import { UserService } from '../../../core/services/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {

  @Input()
  userInfo: UserInfoDTO;

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
