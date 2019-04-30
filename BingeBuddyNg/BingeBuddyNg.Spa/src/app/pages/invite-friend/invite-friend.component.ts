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
  styleUrls: ['./invite-friend.component.scss']
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

  async onInvite() {

    this.isBusy = true;
    this.invitationService.createInvitation().subscribe(token => {
      this.isBusy = false;
      this.invitationToken = token;

    }, e => {
      console.error(e);
      this.isBusy = false;
      this.shellInteractionService.showErrorMessage();
    });
  }

  onCopyToClipboard(el) {
    el.select();
    document.execCommand('copy');

    this.snackbar.open(this.translateService.instant('CopiedToClipboardNowSend'), 'OK');
  }

  onShare() {

    const title = this.translateService.instant('InvitationTitle');
    const text = this.translateService.instant('InvitationText', { userName: this.currentUserProfile.nickname });
    const url = this.getInvitationLink();

    console.log('triggering share ui', title, text, url);

    const nav: any = navigator;

    nav.share({ title, text, url })
      .then(_ => {
        console.log('sharing successful.');
        this.invitationToken = null;
      })
      .catch(error => console.error('Error sharing', error));
  }

  getInvitationLink(): string {
    const url = `https://bingebuddy.azureedge.net/welcome-invited/${this.invitationToken}`;
    return url;
  }

  isSharingSupported(): boolean {
    return navigator['share'];
  }
}
