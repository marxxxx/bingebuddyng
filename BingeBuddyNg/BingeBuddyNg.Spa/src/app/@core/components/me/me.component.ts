import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { AuthService } from '../../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { map, filter, tap } from 'rxjs/operators';
import { UserStatisticsDTO } from 'src/models/UserStatisticsDTO';

@Component({
  selector: 'app-me',
  templateUrl: './me.component.html',
  styleUrls: ['./me.component.css']
})
export class MeComponent implements OnInit {

  userInfo$: Observable<UserInfoDTO>;
  stats: UserStatisticsDTO;

  constructor(private authService: AuthService) { }

  ngOnInit() {
    this.userInfo$ = this.authService.currentUserProfile$
      .pipe(filter(profile => (profile != null)))
      .pipe(
        tap(profile => this.stats = profile.user.currentStats),
        map(profile => ({ userId: profile.sub, userName: profile.nickname }))
        );
  }
}
