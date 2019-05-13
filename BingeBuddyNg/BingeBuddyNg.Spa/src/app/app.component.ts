import { UserProfile } from './../models/UserProfile';
import { User } from '../models/User';
import { UserService } from './core/services/user.service';
import { MatSnackBar } from '@angular/material';
import { AuthService } from './core/services/auth.service';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { SwPush, SwUpdate } from '@angular/service-worker';
import { PushInfo } from '../models/PushInfo';
import { NotificationService } from './core/services/notification.service';
import { Subscription, forkJoin, combineLatest, Observable, from } from 'rxjs';
import { InvitationService } from './invitation/services/invitation.service';
import { SettingsService } from './core/services/settings.service';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  readonly vapidPublicKey = 'BP7M6mvrmwidRr7II8ewUIRSg8n7_mKAlWagRziRRluXnMc_d_rPUoVWGHb79YexnD0olGIFe_xackYqe1fmoxo';
  private pushInfo: PushInfo;
  private sub: Subscription;
  private userProfile: UserProfile;
  private userLanguage: string;

  constructor(
    public auth: AuthService,
    private translate: TranslateService,
    private userService: UserService,
    private snackbar: MatSnackBar,
    private notification: NotificationService,
    private invitationService: InvitationService,
    private settingsService: SettingsService,
    private pushService: SwPush,
    private updateService: SwUpdate
  ) { }

  ngOnInit() {
    if (location.pathname.indexOf('invitation') < 0) {
      this.auth.handleAuthentication(location.pathname);
      this.auth.scheduleRenewal();
    }

    // this language will be used as a fallback when a translation isn't found in the current language
    this.translate.setDefaultLang(this.settingsService.DefaultLanguage);
    this.userLanguage = this.settingsService.getLanguage();

    // the lang to use, if the lang isn't available, it will use the current loader to get them
    this.translate.use(this.userLanguage);

    combineLatest([
      this.auth.currentUserProfile$.pipe(filter(userProfile => userProfile != null)),
      from(this.pushService
        .requestSubscription({
          serverPublicKey: this.vapidPublicKey
        }))
    ]).subscribe(r => {

      this.userProfile = r[0];
      const sub = r[1];
      console.log('Subscription received', sub);

      this.pushInfo = this.getPushInfo(sub);
      this.registerUser(this.pushInfo);
      this.handleInvitations();
    });


    // subscribe to PWA updates
    this.updateService.available.subscribe(e => {
      const message = this.translate.instant('UpdateAvailableMessage');
      this.snackbar
        .open(message, 'OK')
        .onAction()
        .subscribe(r => {
          location.reload(true);
        });
    });

    this.pushService.messages.subscribe((m: any) => {
      if (m.notification && m.notification.body) {
        this.snackbar.open(m.notification.body, 'OK');
        this.notification.raiseActivityReceived();
      }
    });

    this.pushService.notificationClicks.subscribe(event => {
      const url = event.notification.data.url || 'https://bingebuddy.azureedge.net';
      window.open(url);
      console.log('[Service Worker] Notification click Received. event', event);
    });
  }

  // private handleOnboarding() {
  //   if (this.settingsService.getIsOnboarded() === false) {
  //     console.log('going to onboarding ...');
  //     this.router.navigateByUrl('/onboarding');
  //   }
  // }

  ngOnDestroy() {
    if (this.sub) {
      this.sub.unsubscribe();
      this.sub = null;
    }
  }

  handleInvitations() {
    const invitationToken = localStorage.getItem('invitationToken');
    if (invitationToken) {
      console.log('accepting invitation ...', invitationToken);
      this.invitationService.acceptInvitation(invitationToken).subscribe(
        r => {
          console.log('successfully accepted invitation');
          localStorage.removeItem('invitationToken');
        },
        e => {
          console.error('error accepting invitation', e);
        }
      );
    }
  }

  getPushInfo(sub: PushSubscription): PushInfo {
    const subJSObject = JSON.parse(JSON.stringify(sub));

    const pushInfo: PushInfo = {
      subscriptionEndpoint: sub.endpoint,
      auth: subJSObject.keys.auth,
      p256dh: subJSObject.keys.p256dh
    };

    console.log('got push info', pushInfo);
    return pushInfo;
  }

  registerUser(pushInfo: PushInfo) {
    // register user
    const user: User = {
      id: this.userProfile.sub,
      name: this.userProfile.nickname,
      profileImageUrl: this.userProfile.picture,
      pushInfo: pushInfo,
      language: this.userLanguage
    };

    console.log('registering user ...');
    console.log(user);
    this.userService
      .saveUser(user)
      .subscribe(_ => console.log('user registration completed'), e => console.error('error registering user info', e));
  }
}
