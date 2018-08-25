import { UserDTO } from './../models/UserDTO';
import { UserService } from './services/user.service';
import { DataService } from './services/data.service';
import { MatSnackBar } from '@angular/material';
import { Router } from '@angular/router';
import { AuthService } from './services/auth.service';
import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { SwPush, SwUpdate } from '@angular/service-worker';
import { PushInfo } from '../models/PushInfo';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {

  readonly VAPID_PUBLIC_KEY = 'BP7M6mvrmwidRr7II8ewUIRSg8n7_mKAlWagRziRRluXnMc_d_rPUoVWGHb79YexnD0olGIFe_xackYqe1fmoxo';

  // private: 1NKizDYbqdvxaN_su5xvcC3GipJz65hD3UOmYGDFrRw

  constructor(public auth: AuthService, translate: TranslateService, router: Router,
    private userService: UserService,
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

    let pushInfo: PushInfo = null;

    console.log('requesting push subscription ...');

    this.pushService.requestSubscription({
      serverPublicKey: this.VAPID_PUBLIC_KEY
    }).then(sub => {
      console.log('Subscription received');
      console.log(sub);

      pushInfo = this.getPushInfo(sub);
      this.registerUser(pushInfo);
    }).catch(err => {
      console.error(err);
      this.snackbar.open('Failed to register for push notifications', 'OK', { duration: 1000 });
      this.registerUser(null);
    });

    this.pushService.messages.subscribe((m: any) => {
      console.log(m);
      if (m.notification && m.notification.body) {
        this.snackbar.open(m.notification.body, 'OK');
      }
      // this.dataService.setBeerRequestApprovalResponse(m.notification.result);
      console.log(m);
    });
  }

  getPushInfo(sub: PushSubscription): PushInfo {

    const subJSObject = JSON.parse(JSON.stringify(sub));

    const pushInfo: PushInfo = {
      subscriptionEndpoint: sub.endpoint,
      auth: subJSObject.keys.auth,
      p256dh: subJSObject.keys.p256dh
    };

    console.log('got push info');
    console.log(pushInfo);
    return pushInfo;
  }

  registerUser(pushInfo: PushInfo) {

    if (this.auth.isAuthenticated()) {

      this.auth.getProfile((err, profile) => {
        // register user
        const user: UserDTO = {
          id: profile.sub,
          name: profile.nickname,
          profileImageUrl: profile.picture,
          pushInfo: pushInfo
        };

        console.log('registering user ...');
        console.log(user);
        this.userService.saveUser(user).subscribe(_ => console.log('user registration completed'));
      });
    } else {
      console.warn('user not authenticated');
    }
  }
}


