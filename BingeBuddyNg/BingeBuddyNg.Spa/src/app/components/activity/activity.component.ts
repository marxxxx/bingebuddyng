import { UserInfo } from './../../../models/UserInfo';
import { ActivityType } from './../../../models/ActivityType';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { Component, OnInit, Input, ViewChildren, EventEmitter, Output } from '@angular/core';
import { TranslateService } from '../../../../node_modules/@ngx-translate/core';
import { ActivityService } from '../../services/activity.service';
import { ReactionDTO } from '../../../models/ReactionDTO';
import { AuthService } from '../../services/auth.service';
import { ReactionType } from '../../../models/ReactionType';
import { Reaction } from '../../../models/Reaction';
import { CommentReaction } from '../../../models/CommentReaction';
import { UserService } from 'src/app/services/user.service';
import { MatTooltip } from '@angular/material';
import { Router } from '@angular/router';


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
  comment: string;
  get userInfo(): UserInfo {
    let userInfo: UserInfo = null;
    if (this.activity && this.activity.activity) {
      userInfo = {
        userId: this.activity.activity.userId,
        userName: this.activity.activity.userName
      };
    }

    return userInfo;
  }


  @Input()
  activity: ActivityStatsDTO;

  @Input()
  currentUser: UserInfo;

  @Output()
  commentOpenChanged: EventEmitter<boolean> = new EventEmitter<boolean>();

  @ViewChildren(MatTooltip)
  tooltips: MatTooltip[];


  constructor(private translate: TranslateService,
    private router: Router,
    private activityService: ActivityService,
    public userService: UserService) { }

  ngOnInit() {
  }

  isDrinkActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Drink;
  }

  isMessageActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Message;
  }

  isImageActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Image && !this.isVideoActivity();
  }

  isVideoActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Image &&
      this.activity.activity.imageUrl && this.activity.activity.imageUrl.endsWith('mp4');
  }

  getDrinkMessage(): string {
    if (!this.activity.activity.drinkName) {
      return this.activity.activity.message;
    }
    const message = this.translate.instant('IJustHadA',
      { value: this.translate.instant(this.activity.activity.drinkName) });
    return message;
  }

  onShowTooltips() {
    this.tooltips.forEach(t => t.show());
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
      this.setCommentVisible(false);
    }, e => {
      this.setCommentVisible(false);
      console.error(e);
    });
  }

  onKeydown(ev) {
    if (ev.key === 'Enter') {
      this.onComment();
    }
  }

  onLocationClick() {
    this.router.navigate(['bingemap'], { queryParams: { selectedActivityId: this.activity.activity.id } });
  }

  onCommentClicked(ev) {
    this.setCommentVisible(!this.isCommentVisible);
    this.tooltips.forEach(t => t.hide());
    ev.preventDefault();
    ev.stopPropagation();
  }

  setCommentVisible(isVisible: boolean) {
    this.isCommentVisible = isVisible;
    this.commentOpenChanged.emit(this.isCommentVisible);
  }

  createReactionDTO(type: ReactionType, comment?: string): ReactionDTO {
    const reaction: ReactionDTO = { activityId: this.activity.activity.id, type: type, comment: comment };
    return reaction;
  }


  createReaction(type: ReactionType): Reaction {
    const reaction: Reaction = {
      userId: this.currentUser.userId,
      userName: this.currentUser.userName,
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
    if (this.currentUser == null || this.activity == null || this.activity.activity == null || this.activity.activity.likes == null) {
      return false;
    }
    const result = this.activity.activity.likes.filter(l => l.userId === this.currentUser.userId).length > 0;
    return result;
  }

  isCheeredByMe(): boolean {
    if (!this.hasData() || this.activity.activity.cheers == null) {
      return false;
    }
    const result = this.activity.activity.cheers.filter(l => l.userId === this.currentUser.userId).length > 0;
    return result;
  }

  getLikeUserNames(): string {
    if (!this.hasData() || this.activity.activity.likes == null) {
      return null;
    }
    return this.activity.activity.likes.map(l => l.userName).join(',');
  }

  getCheersUserNames(): string {
    if (!this.hasData() || this.activity.activity.cheers == null) {
      return null;
    }
    return this.activity.activity.cheers.map(l => l.userName).join(',');
  }

  isMessageLink(link: string): boolean {
    return (link && (link.indexOf('http') === 0));
  }

  hasData(): boolean {
    const hasData = (this.currentUser != null && this.activity != null && this.activity.activity != null);
    return hasData;
  }
}