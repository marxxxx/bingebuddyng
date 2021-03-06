import { Router } from '@angular/router';
import { AuthService } from '../../../@core/services/auth/auth.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { filter } from 'rxjs/operators';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-welcome',
  templateUrl: './welcome.component.html',
  styleUrls: ['./welcome.component.css']
})
export class WelcomeComponent implements OnInit, OnDestroy {

  private sub: Subscription;

  constructor(public auth: AuthService,
    private router: Router) { }

  ngOnInit() {
    this.sub = this.auth.currentUserProfile$
      .pipe(filter(p => p != null))
      .subscribe(() => {
        this.router.navigate(['/activity-feed']);
      });
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

  onLogin() {
    if (!this.auth.isAuthenticated()) {
      this.auth.login();
    }
  }
}
