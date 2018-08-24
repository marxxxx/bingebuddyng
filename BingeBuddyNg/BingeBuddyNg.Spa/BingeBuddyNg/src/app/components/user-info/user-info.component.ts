import { UserDTO } from '../../../models/UserDTO';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-info',
  templateUrl: './user-info.component.html',
  styleUrls: ['./user-info.component.css']
})
export class UserInfoComponent implements OnInit {
  profile: any;

  constructor(private authService: AuthService, private userService: UserService) { }

  ngOnInit() {

    if (this.authService.isAuthenticated()) {
      this.loadProfile();
    }


    this.authService.isLoggedIn$.subscribe(isLoggedIn => {
      console.log('UserProfileComponent ' + isLoggedIn);
      if (isLoggedIn) {
        this.loadProfile();
      } else {
        this.profile = null;
      }
    });

  }

  loadProfile() {
    this.authService.getProfile((err, profile) => {
      this.profile = profile;
    });
  }

}
