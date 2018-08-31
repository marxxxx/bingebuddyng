import { ActivityType } from './../../../models/ActivityType';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { Component, OnInit, Input } from '@angular/core';
import { TranslateService } from '../../../../node_modules/@ngx-translate/core';
import { ActivityService } from '../../services/activity.service';
import { ReactionDTO } from '../../../models/ReactionDTO';
import { UserDTO } from '../../../models/UserDTO';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { ReactionType } from '../../../models/ReactionType';

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.scss']
})
export class ActivityComponent implements OnInit {

  isBusyLiking = false;
  isBusyCheering = false;
  user: UserDTO;

  @Input()
  activity: ActivityStatsDTO;


  constructor(private translate: TranslateService,
    private auth: AuthService,
    private activityService: ActivityService) { }

  ngOnInit() {
    if (this.auth.isAuthenticated()) {
      this.auth.getProfile((err, profile) => {
        this.user = { id: profile.sub, profileImageUrl: profile.picture, name: profile.nickname };
      });
    }

  }

  isDrinkActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Drink;
  }

  isMessageActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Message;
  }

  getDrinkMessage(): string {
    if (!this.activity.activity.drinkName) {
      return this.activity.activity.message;
    }
    const message = this.translate.instant('IJustHadA',
      { value: this.translate.instant(this.activity.activity.drinkName) });
    return message;
  }

  onLike() {

    this.isBusyLiking = true;
    const reaction: ReactionDTO = { activityId: this.activity.activity.id, type: ReactionType.Like };
    this.activityService.addReaction(reaction).subscribe(r => {
      this.isBusyLiking = false;
      this.activity.activity.likes.push(
        {
          userId: this.user.id,
          userName: this.user.name,
          userProfileImageUrl: this.user.profileImageUrl,
          timestamp: new Date(),
          type: ReactionType.Like
        });
    });



  }

  onCheers() {

    this.isBusyCheering = true;
    const reaction: ReactionDTO = { activityId: this.activity.activity.id, type: ReactionType.Cheers };
    this.activityService.addReaction(reaction).subscribe(r => {
      this.isBusyCheering = false;
      this.activity.activity.cheers.push(
        {
          userId: this.user.id,
          userName: this.user.name,
          userProfileImageUrl: this.user.profileImageUrl,
          timestamp: new Date(),
          type: ReactionType.Cheers
        });
    });
  }
}
