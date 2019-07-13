import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { AuthService } from '../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map, filter } from 'rxjs/operators';

@Component({
  selector: 'app-me',
  templateUrl: './me.component.html',
  styleUrls: ['./me.component.css']
})
export class MeComponent implements OnInit {

  userInfo$: Observable<UserInfoDTO>;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.userInfo$ = this.authService.currentUserProfile$
      .pipe(filter(profile => (profile != null)))
      .pipe(map(profile => ({ userId: profile.sub, userName: profile.nickname })));
  }
}
