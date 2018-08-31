import { ActivityType } from './../../../models/ActivityType';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { Component, OnInit, Input } from '@angular/core';
import { TranslateService } from '../../../../node_modules/@ngx-translate/core';
import { ActivityService } from '../../services/activity.service';
import { ReactionDTO } from '../../../models/ReactionDTO';
import { UserDTO } from '../../../models/UserDTO';
import { AuthService } from '../../services/auth.service';
import { ReactionType } from '../../../models/ReactionType';
import { Reaction } from '../../../models/Reaction';

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
      const addedLike: Reaction = {
        userId: this.user.id,
        userName: this.user.name,
        userProfileImageUrl: this.user.profileImageUrl,
        timestamp: new Date(),
        type: ReactionType.Like
      };
      this.activity.activity.likes.push(addedLike);
      this.activity.activity.likes = this.activity.activity.likes;
    });



  }

  onCheers() {

    this.isBusyCheering = true;
    const reaction: ReactionDTO = { activityId: this.activity.activity.id, type: ReactionType.Cheers };
    this.activityService.addReaction(reaction).subscribe(r => {
      this.isBusyCheering = false;
      const addedCheers: Reaction = {
        userId: this.user.id,
        userName: this.user.name,
        userProfileImageUrl: this.user.profileImageUrl,
        timestamp: new Date(),
        type: ReactionType.Cheers
      };
      this.activity.activity.cheers.push(addedCheers);
      this.activity.activity.cheers = this.activity.activity.cheers;
    }, e => {
      console.error(e);
    });
  }

  isLikedByMe(): boolean {
    const result = this.activity.activity.likes.filter(l => l.userId === this.user.id).length > 0;
    return result;
  }

  isCheeredByMe(): boolean {
    const result = this.activity.activity.cheers.filter(l => l.userId === this.user.id).length > 0;
    return result;
  }
}
