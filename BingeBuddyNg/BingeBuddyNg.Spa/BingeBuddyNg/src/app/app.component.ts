import { DataService } from './services/data.service';
import { MatSnackBar } from '@angular/material';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { SwPush, SwUpdate } from '@angular/service-worker';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  readonly VAPID_PUBLIC_KEY = 'BP7M6mvrmwidRr7II8ewUIRSg8n7_mKAlWagRziRRluXnMc_d_rPUoVWGHb79YexnD0olGIFe_xackYqe1fmoxo';

  // private: 1NKizDYbqdvxaN_su5xvcC3GipJz65hD3UOmYGDFrRw

  constructor(public auth: AuthService, translate: TranslateService, router: Router,
    private dataService: DataService,
    private snackbar: MatSnackBar,
    private pushService: SwPush,
    private updateService: SwUpdate) {
    auth.handleAuthentication();

    // this language will be used as a fallback when a translation isn't found in the current language
    translate.setDefaultLang('en');

    // the lang to use, if the lang isn't available, it will use the current loader to get them
    translate.use(navigator.language.substr(0, 2));
  }

  ngOnInit() {

    this.updateService.available.subscribe(e => {
      this.snackbar.open(`Update available! Press F5 to install.`, 'OK');
    });

    this.updateService.activated.subscribe(e => {
      this.snackbar.open(`Update ${e.current} activated!`, 'OK', { duration: 3000 });
    });

    this.pushService.requestSubscription({
      serverPublicKey: this.VAPID_PUBLIC_KEY
    }).then(sub => {
      console.log('Subscription received');
      console.log(sub);
      //this.dataService.setPushSubscription(sub);

      console.log(sub.toJSON());
      console.log(sub.getKey('p256dh'));
    }).catch(err => {
      console.error(err);
      this.snackbar.open('Failed to register for push notifications', 'OK', {duration: 1000});
    });

    this.pushService.messages.subscribe((m: any) => {
      this.snackbar.open(m.notification.body, 'OK');
      // this.dataService.setBeerRequestApprovalResponse(m.notification.result);
      console.log(m);
    });
  }
}


