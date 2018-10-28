import { UserInfo } from '../../../models/UserInfo';
import { AuthService } from '../../services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-me',
  templateUrl: './me.component.html',
  styleUrls: ['./me.component.css']
})
export class MeComponent implements OnInit {

  userInfo: UserInfo;

  constructor(private authService: AuthService) { }

  ngOnInit() {

    if (this.authService.isLoggedIn$.value === true) {
      this.loadProfile();
    }

    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      if (isLoggedIn) {
        this.loadProfile();
      } else {
        this.userInfo = null;
      }
    });

  }

  loadProfile() {
    this.authService.getProfile((err, profile) => {
      if (profile) {
        this.userInfo = { userId: profile.sub, userName: profile.nickname, userProfileImageUrl: profile.picture };
        console.log('loaded userinfo');
        console.log(this.userInfo);
      } else {
        console.log('no profile');
        console.error(err);
      }
    });
  }

}
