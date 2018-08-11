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
    if (this.authService.userProfile) {
      console.log('having profile');
      this.profile = this.authService.userProfile;
    } else {
      this.authService.getProfile((err, profile) => {
        this.profile = profile;
        console.log('got profile');
        console.log(this.profile);

        // register user
        const user: UserDTO = {
          id: profile.sub,
          name: profile.nickname,
          profileImageUrl: profile.picture
        };
        this.userService.saveUser(user).subscribe( _ => console.log('user registration completed'));
      });
    }
  }

}
