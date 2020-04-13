import { Router, ActivatedRoute } from '@angular/router';
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
    console.log('WelcomeComponent: ngOnInit');
    this.sub = this.auth.currentUserProfile$
      .pipe(filter(p => p != null))
      .subscribe(() => {
        console.log('WelcomeComponent: Navigating');
        this.router.navigate(['/activity-feed']);
      });
  }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
    }
  }

  onLogin() {
    this.auth.login();
  }
}
