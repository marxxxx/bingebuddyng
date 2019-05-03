import { Router } from '@angular/router';
import { AuthService } from '../../core/auth.service';
import { Component, OnInit } from '@angular/core';
import { ShellInteractionService } from '../../services/shell-interaction.service';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
})
export class WelcomeComponent implements OnInit {

  constructor(public auth: AuthService,
    private router: Router) { }

  ngOnInit() {
    if (this.auth.isAuthenticated()) {
      this.router.navigate(['/activity-feed']);
    }
  }


  onLogin() {
    this.auth.login();
  }
}
