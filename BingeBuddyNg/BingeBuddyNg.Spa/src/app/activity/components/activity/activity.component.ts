import { Component, OnInit, Input, EventEmitter, Output, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { filter, finalize } from 'rxjs/operators';

import { ReactionDialogComponent } from './../reaction-dialog/reaction-dialog.component';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { ActivityType } from '../../../../models/ActivityType';
import { ActivityStatsDTO } from '../../../../models/ActivityStatsDTO';

import { ActivityService } from './../../services/activity.service';
import { AddReactionDTO } from '../../../../models/AddReactionDTO';
import { ReactionType } from '../../../../models/ReactionType';
import { UserService } from 'src/app/@core/services/user.service';
import { ShellInteractionService } from 'src/app/@core/services/shell-interaction.service';


@Component({
  selector: 'app-activity',
  templateUrl: './activity.component.html',
  styleUrls: ['./activity.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ActivityComponent implements OnInit {

  ActivityType = ActivityType;
  isBusyLiking = false;
  isBusyCheering = false;
  isBusyCommenting = false;
  isCommentVisible = false;
  comment: string;
  get userInfo(): UserInfoDTO {
    let userInfo: UserInfoDTO = null;
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
  currentUser: UserInfoDTO;

  @Input()
  isHighlighted: boolean;

  @Output()
  commentOpenChanged: EventEmitter<boolean> = new EventEmitter<boolean>();

  @Output()
  deleted = new EventEmitter();

  constructor(
    private router: Router,
    private changeDetector: ChangeDetectorRef,
    private activityService: ActivityService,
    private dialog: MatDialog,
    private shellInteraction: ShellInteractionService,
    public userService: UserService) { }

  ngOnInit() {
  }

  isImageActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Image && !this.isVideoActivity();
  }

  isVideoActivity(): boolean {
    return this.activity.activity.activityType === ActivityType.Image &&
      this.activity.activity.imageUrl && this.activity.activity.imageUrl.endsWith('mp4');
  }

  onShowReactions() {
    this.dialog.open(ReactionDialogComponent, { width: '95%', data: this.activity.activity });
  }

  onLike(ev) {
    ev.stopPropagation();

    this.isBusyLiking = true;
    const reaction = this.createAddReactionDTO(ReactionType.Like);
    this.activityService.addReaction(this.activity.activity.id, reaction)
      .pipe(finalize(() => {
        this.isBusyLiking = false;
        this.changeDetector.detectChanges();
      }))
      .subscribe(() => {
        this.activity.activity.likes.push({
          type: ReactionType.Like,
          timestamp: new Date(),
          userId: this.currentUser.userId,
          userName: this.currentUser.userName
        });
      }, e => {
        console.error(e);
      });
  }

  onCheers(ev) {
    ev.stopPropagation();
    this.isBusyCheering = true;
    const reaction = this.createAddReactionDTO(ReactionType.Cheers);
    this.activityService.addReaction(this.activity.activity.id, reaction)
      .pipe(finalize(() => {
        this.isBusyCheering = false;
        this.changeDetector.detectChanges();
      }))
      .subscribe(() => {
        this.activity.activity.cheers.push({
          type: ReactionType.Cheers,
          timestamp: new Date(),
          userId: this.currentUser.userId,
          userName: this.currentUser.userName
        });
      }, e => {
        console.error(e);
      });
  }

  onComment() {
    this.isBusyCommenting = true;
    this.changeDetector.detectChanges();
    const reaction = this.createAddReactionDTO(ReactionType.Comment, this.comment);
    this.activityService.addReaction(this.activity.activity.id, reaction)
      .pipe(finalize(() => {
        this.isBusyCommenting = false;
        this.changeDetector.detectChanges();
      }))
      .subscribe(() => {
        this.activity.activity.comments.push({
          type: ReactionType.Comment,
          timestamp: new Date(),
          userId: this.currentUser.userId,
          userName: this.currentUser.userName,
          comment: this.comment
        });
        this.comment = null;
        this.setCommentVisible(false);
      }, e => {
        this.setCommentVisible(false);
        console.error(e);
      });
  }

  onDelete() {
    this.shellInteraction.showConfirmationDialog({
      icon: 'delete',
      title: 'DeleteActivity',
      message: 'ReallyDeleteActivity',
      confirmButtonCaption: 'Delete',
      cancelButtonCaption: 'Cancel'
    }).pipe(filter(isConfirmed => isConfirmed))
      .subscribe(() => {
        console.log('deleting activity', this.activity.activity.id);
        this.activityService.deleteActivity(this.activity.activity.id).subscribe(() => this.deleted.emit(), e => {
          this.shellInteraction.showErrorMessage();
          console.error(e);
        });
      });
  }

  onLocationClick() {
    this.router.navigate(['bingemap'], { queryParams: { selectedActivityId: this.activity.activity.id } });
  }

  onCommentClicked(ev) {
    this.setCommentVisible(!this.isCommentVisible);
    ev.preventDefault();
    ev.stopPropagation();
  }

  setCommentVisible(isVisible: boolean) {
    this.isCommentVisible = isVisible;
    this.commentOpenChanged.emit(this.isCommentVisible);
  }

  createAddReactionDTO(type: ReactionType, comment?: string): AddReactionDTO {
    const reaction: AddReactionDTO = { type: type, comment: comment };
    return reaction;
  }

  createAddCommentReaction(comment: string): AddReactionDTO {
    const reaction: AddReactionDTO = this.createAddReactionDTO(ReactionType.Comment);
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

  isMessageLink(link: string): boolean {
    return (link && (link.indexOf('http') === 0));
  }

  hasData(): boolean {
    const hasData = (this.currentUser != null && this.activity != null && this.activity.activity != null);
    return hasData;
  }
}
