import { TranslateService } from '@ngx-translate/core';
import { AuthService } from './../../services/auth.service';
import { ActivatedRoute, Router } from '@angular/router';
import { InvitationService } from './../../services/invitation.service';
import { Component, OnInit } from '@angular/core';
import { InvitationInfo } from 'src/models/InvitationInfo';
import { Observable } from 'rxjs';
import { MatSnackBar } from '@angular/material';

@Component({
  selector: 'app-welcome-invited',
  templateUrl: './welcome-invited.component.html',
  styleUrls: ['./welcome-invited.component.css']
})
export class WelcomeInvitedComponent implements OnInit {

  invitationToken: string;
  invitationInfo: InvitationInfo;
  isBusy = false;

  constructor(private invitationService: InvitationService,
    private auth: AuthService,
    private route: ActivatedRoute,
    private router: Router,
    private snackbar: MatSnackBar,
    private translateService: TranslateService) { }

  ngOnInit() {
    this.route.paramMap.subscribe(p => {

      this.invitationToken = p.get('invitationToken');
      localStorage.setItem('invitationToken', this.invitationToken);

      this.isBusy = true;
      this.invitationService.getInvitationInfo(this.invitationToken).subscribe(r => {
        this.isBusy = false;
        this.invitationInfo = r;
        if (r.invitation.acceptingUserId !== null) {
          this.snackbar.open(this.translateService.instant('InvitationAccepted'), 'OK', { duration: 1000 });
          this.router.navigate(['/welcome']);
        }
      }, e => {
        console.error('failed to load invitation info', e);
        this.router.navigate(['/welcome']);
      });
    });
  }

  onLogin() {
    this.auth.login();
  }
}
