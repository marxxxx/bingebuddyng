import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { AuthService } from '../../services/auth/auth.service';
import { Component, OnInit } from '@angular/core';
import { merge, Observable } from 'rxjs';
import { map, filter, tap } from 'rxjs/operators';
import { UserStatisticsDTO } from 'src/models/UserStatisticsDTO';
import { StateService } from '../../services/state.service';

@Component({
  selector: 'app-me',
  templateUrl: './me.component.html',
  styleUrls: ['./me.component.css'],
})
export class MeComponent implements OnInit {
  userInfo$: Observable<UserInfoDTO>;
  stats$: Observable<UserStatisticsDTO>;
  currentUserId: string;

  constructor(private authService: AuthService, private stateService: StateService) {}

  ngOnInit() {
    this.userInfo$ = this.authService.currentUserProfile$
      .pipe(filter((profile) => profile != null))
      .pipe(tap(profile => this.currentUserId = profile.sub))
      .pipe(map((profile) => ({ userId: profile.sub, userName: profile.nickname })));

    this.stats$ = merge(
      this.authService.currentUserProfile$
        .pipe(filter((profile) => profile != null))
        .pipe(map((profile) => profile.user.currentStats)),
      this.stateService.activityReceived$
        .pipe(filter((a) => a?.userStats?.userId === this.currentUserId))
        .pipe(map((a) => a.userStats))
    );
  }
}
