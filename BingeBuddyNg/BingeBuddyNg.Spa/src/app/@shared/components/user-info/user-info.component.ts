import { Router } from '@angular/router';
import { UserService } from '../../../@core/services/user.service';
import { Component, OnInit, Input } from '@angular/core';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { UserStatisticsDTO } from 'src/models/UserStatisticsDTO';

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

  @Input()
  stats: UserStatisticsDTO;

  constructor(private userService: UserService) { }

  ngOnInit() {
  }

  getProfileImageUrl() {
    if (this.userInfo == null) {
      return null;
    }
    return this.userService.getProfileImageUrl(this.userInfo.userId);
  }

  getFilter(): string {
    if (!this.stats?.currentAlcoholization) {
      return null;
    }
    return `blur(${this.stats.currentAlcoholization}px)`;
  }

  getTransform(): string {
    if (!this.stats?.currentAlcoholization) {
      return null;
    }
    return `rotate(${this.stats.currentAlcoholization * 30}deg)`;
  }
}
