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
import { CommentReaction } from '../../../models/CommentReaction';

@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.scss']
})
export class ActivityComponent implements OnInit {

  isBusyLiking = false;
  isBusyCheering = false;
  isBusyCommenting = false;
  isCommentVisible = false;
  user: UserDTO;
  comment: string;

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

  isImageActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Image;
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
    const reaction = this.createReactionDTO(ReactionType.Like);
    this.activityService.addReaction(reaction).subscribe(r => {
      this.isBusyLiking = false;
      const addedLike = this.createReaction(ReactionType.Like);
      this.activity.activity.likes.push(addedLike);
    }, e => {
      this.isBusyLiking = false;
      console.error(e);

    });

  }

  onCheers() {

    this.isBusyCheering = true;
    const reaction = this.createReactionDTO(ReactionType.Cheers);
    this.activityService.addReaction(reaction).subscribe(r => {
      this.isBusyCheering = false;
      const addedCheers = this.createReaction(ReactionType.Cheers);
      this.activity.activity.cheers.push(addedCheers);
    }, e => {
      this.isBusyCheering = false;
      console.error(e);
    });
  }

  onComment() {
    this.isBusyCommenting = true;
    const reaction = this.createReactionDTO(ReactionType.Comment, this.comment);
    this.activityService.addReaction(reaction).subscribe(r => {
      this.isBusyCommenting = false;
      const addedComment = this.createCommentReaction(this.comment);
      this.activity.activity.comments.push(addedComment);
      this.comment = null;
      this.isCommentVisible = false;
    }, e => {
      this.isBusyCommenting = false;
      console.error(e);
    });
  }

  onKeydown(ev) {
    if (ev.key === 'Enter') {
      this.onComment();
    }
  }

  createReactionDTO(type: ReactionType, comment?: string): ReactionDTO {
    const reaction: ReactionDTO = { activityId: this.activity.activity.id, type: type, comment: comment };
    return reaction;
  }


  createReaction(type: ReactionType): Reaction {
    const reaction: Reaction = {
      userId: this.user.id,
      userName: this.user.name,
      userProfileImageUrl: this.user.profileImageUrl,
      timestamp: new Date(),
      type: type
    };
    return reaction;
  }

  createCommentReaction(comment: string): CommentReaction {
    const reaction: CommentReaction = this.createReaction(ReactionType.Comment);
    reaction.comment = comment;
    return reaction;
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
