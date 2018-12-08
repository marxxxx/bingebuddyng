import { AuthService } from './../../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { InvitationService } from './../../services/invitation.service';
import { Component, OnInit } from '@angular/core';
import { InvitationInfo } from 'src/models/InvitationInfo';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-welcome-invited',
  templateUrl: './welcome-invited.component.html',
  styleUrls: ['./welcome-invited.component.css']
})
export class WelcomeInvitedComponent implements OnInit {

  invitationToken: string;
  invitationInfo$: Observable<InvitationInfo>;

  constructor(private invitationService: InvitationService,
    private auth: AuthService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.route.paramMap.subscribe(p => {

      this.invitationToken = p.get('invitationToken');
      localStorage.setItem('invitationToken', this.invitationToken);

      this.invitationInfo$ = this.invitationService.getInvitationInfo(this.invitationToken);
    });
  }

  onLogin() {
    this.auth.login();
  }
}
