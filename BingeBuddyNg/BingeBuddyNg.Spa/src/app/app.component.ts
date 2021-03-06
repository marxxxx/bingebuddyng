import { Component, OnInit, OnDestroy } from '@angular/core';
import { SwPush, SwUpdate } from '@angular/service-worker';
import { MatSnackBar } from '@angular/material/snack-bar';
import { TranslocoService } from '@ngneat/transloco';
import { Subscription, combineLatest, from } from 'rxjs';
import { filter } from 'rxjs/operators';

import { UserProfile } from './../models/UserProfile';
import { UserService } from './@core/services/user.service';
import { AuthService } from './@core/services/auth/auth.service';
import { PushInfo } from '../models/PushInfo';
import { NotificationService } from './@core/services/notification.service';
import { InvitationService } from './invitation/services/invitation.service';
import { SettingsService } from './@core/services/settings.service';
import { CreateOrUpdateUserDTO } from 'src/models/CreateOrUpdateUserDTO';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';
import { GameStartedMessage } from './game/models/GameStartedMessage';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit, OnDestroy {
  private pushInfo: PushInfo;
  private sub: Subscription = new Subscription();
  private userProfile: UserProfile;
  private userLanguage: string;

  constructor(
    public auth: AuthService,
    private translate: TranslocoService,
    private userService: UserService,
    private snackbar: MatSnackBar,
    private notification: NotificationService,
    private invitationService: InvitationService,
    private settingsService: SettingsService,
    private pushService: SwPush,
    private updateService: SwUpdate,
    private router: Router
  ) { }

  ngOnInit() {
    // this language will be used as a fallback when a translation isn't found in the current language
    this.initLanguage();

    // checking invitation
    if (location.pathname.indexOf('invitation') < 0) {
      this.auth.handleAuthentication(location.pathname + location.search);
      this.auth.scheduleRenewal();
    }

    console.log('ngOnInit - passed invitation check');

    this.sub.add(combineLatest([
      this.auth.currentUserProfile$.pipe(filter(userProfile => userProfile != null)),
      from(this.pushService
        .requestSubscription({
          serverPublicKey: environment.credentials.vapidPublicKey
        }))
    ]).subscribe(([profile, pushSubscription]) => {
      console.log('ngOnInit - got user and subscription', profile, pushSubscription);

      this.userProfile = profile;
      console.log('Subscription received', pushSubscription);

      this.pushInfo = this.getPushInfo(pushSubscription);
      this.registerUser(this.pushInfo);

    }));

    this.handleInvitations();

    console.log('ngOnInit - before subscribing to updates');
    // subscribe to PWA updates
    this.sub.add(this.updateService.available.subscribe(e => {
      const message = this.translate.translate('UpdateAvailableMessage');
      this.snackbar
        .open(message, 'OK')
        .onAction()
        .subscribe(r => {
          location.reload();
        });
    }));

    // we need to be logged in before subscribing to the signalR endpoint
    // because this will need an authenticated user in to manage personalized connections
    this.sub.add(this.auth.isLoggedIn$.pipe(filter(isLoggedIn => isLoggedIn))
      .subscribe(() => {
        this.notification.start().then(s => {
          console.log('notification registered');

          this.notification.on('GameStarted', (payload: GameStartedMessage) => {
            console.log('GameStarted', payload);
            this.router.navigate(['/game/play', payload.gameId]);
          });
        });
      }));
  }


  ngOnDestroy() {
    this.sub.unsubscribe();
  }


  private initLanguage() {
    this.translate.setDefaultLang(this.settingsService.DefaultLanguage);
    this.userLanguage = this.settingsService.getLanguage();
    // the lang to use, if the lang isn't available, it will use the current loader to get them
    this.translate.setActiveLang(this.userLanguage);
  }

  handleInvitations() {
    console.log('handleInvitations');
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
    if (!sub) {
      return null;
    }

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
    const user = new CreateOrUpdateUserDTO(
      this.userProfile.sub,
      this.userProfile.nickname,
      this.userProfile.picture,
      pushInfo,
      this.userLanguage);

    console.log('registering user ...');
    console.log(user);
    this.userService
      .createOrUpdateUser(user)
      .subscribe(() => console.log('user registration completed'), e => console.error('error registering user info', e));
  }
}
