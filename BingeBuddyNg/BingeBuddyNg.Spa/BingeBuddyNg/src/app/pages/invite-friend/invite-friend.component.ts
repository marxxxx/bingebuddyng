import { UserProfile } from './../../../models/UserProfile';
import { AuthService } from './../../services/auth.service';
import { ShellInteractionService } from './../../services/shell-interaction.service';
import { Component, OnInit } from '@angular/core';
import { InvitationService } from 'src/app/services/invitation.service';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-invite-friend',
  templateUrl: './invite-friend.component.html',
  styleUrls: ['./invite-friend.component.css']
})
export class InviteFriendComponent implements OnInit {

  isBusy = false;
  invitationToken: string;
  currentUserProfile: UserProfile;
  navigatorInstance: any = navigator;

  constructor(private invitationService: InvitationService,
    private shellInteractionService: ShellInteractionService,
    private authService: AuthService,
    private translateService: TranslateService,
    private snackbar: MatSnackBar) { }

  ngOnInit() {
    this.authService.currentUserProfile$.subscribe(p => this.currentUserProfile = p);
  }

  onInvite() {

    this.isBusy = true;
    this.invitationService.createInvitation().subscribe(token => {
      this.isBusy = false;
      this.invitationToken = token;

      if (this.navigatorInstance.share) {
        this.onShare(token);
      } else {

        console.warn('sharing not available');

      }

    }, e => {
      console.error(e);
      this.isBusy = false;
      this.shellInteractionService.showErrorMessage();
    });
  }

  onCopyToClipboard(el) {
    el.select();
    document.execCommand('copy');

    this.snackbar.open(this.translateService.instant('CopiedToClipboard'), null, {
      duration: 1000
    });
  }

  onShare(token: string) {


    this.navigatorInstance.share({
      title: this.translateService.instant('InvitationTitle'),
      text: this.translateService.instant('InvitationText', { userName: this.currentUserProfile.nickname }),
      url: this.getInvitationLink(),
    })
      .then(() => console.log('Successful share'))
      .catch((error) => {
        console.log('Error sharing', error);
        this.shellInteractionService.showErrorMessage();
      });

  }

  getInvitationLink(): string {
    const url = `https://bingebuddy.azureedge.net/welcome-invited/${this.invitationToken}`;
    return url;
  }
}
