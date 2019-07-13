import { UserProfile } from '../../../../models/UserProfile';
import { AuthService } from '../../../@core/services/auth.service';
import { ShellInteractionService } from '../../../@core/services/shell-interaction.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { InvitationService } from 'src/app/invitation/services/invitation.service';
import { TranslateService } from '@ngx-translate/core';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-invite-friend',
  templateUrl: './invite-friend.component.html',
  styleUrls: ['./invite-friend.component.scss']
})
export class InviteFriendComponent implements OnInit, OnDestroy {

  isBusy = false;
  invitationToken: string;
  currentUserProfile: UserProfile;
  navigatorInstance: any = navigator;
  subscriptions: Subscription[] = [];

  constructor(private invitationService: InvitationService,
    private shellInteractionService: ShellInteractionService,
    private authService: AuthService,
    private translateService: TranslateService,
    private snackbar: MatSnackBar) { }

  ngOnInit() {
    this.subscriptions.push(this.authService.currentUserProfile$.pipe(filter(p => p != null)).subscribe(p => this.currentUserProfile = p));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
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
    const url = `https://bingebuddy.azureedge.net/invitation/welcome/${this.invitationToken}`;
    return url;
  }

  isSharingSupported(): boolean {
    return navigator['share'];
  }
}
