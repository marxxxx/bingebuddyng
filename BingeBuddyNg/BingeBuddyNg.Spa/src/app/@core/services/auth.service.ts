import { CreateOrUpdateUserDTO } from 'src/models/CreateOrUpdateUserDTO';
import { UserService } from 'src/app/@core/services/user.service';
import { Injectable } from '@angular/core';
import * as auth0 from 'auth0-js';
import { Router } from '@angular/router';
import { UserProfile } from '../../../models/UserProfile';
import { BehaviorSubject, Observable, Subscription, of, timer } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import { UserDTO } from 'src/models/UserDTO';
import { SettingsService } from './settings.service';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userProfile: UserProfile;
  refreshSubscription: Subscription;

  readonly idTokenStorageKey = 'id_token';
  readonly tokenStorageKey = 'token';
  readonly expiresAtStorageKey = 'expires_at';


  auth0 = new auth0.WebAuth({
    clientID: '97ShzYXlNSZRiRgoiTR5Ui1JyDs8KxcY',
    domain: 'bingebuddy.eu.auth0.com',
    responseType: 'token id_token',
    audience: 'https://bingebuddyngapi',
    redirectUri: location.protocol + '//' + location.host + '/callback',
    scope: 'openid profile'
  });

  isLoggedIn$: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  currentUserProfile$: BehaviorSubject<UserProfile> = new BehaviorSubject<UserProfile>(null);

  constructor(public router: Router, private userService: UserService, private settings: SettingsService) { }

  public login(): void {
    this.auth0.authorize();
  }

  public handleAuthentication(returnUrl?: string): void {

    console.log('handling authentication', returnUrl);
    const successRoute = (returnUrl != null && returnUrl !== '/callback') ? returnUrl :
      this.settings.getIsOnboarded() ? returnUrl : '/onboarding';
    returnUrl = returnUrl || '/';

    this.auth0.parseHash((err, authResult) => {
      console.log('parseHash completed');
      console.log(authResult);
      console.log(err);
      if (authResult && authResult.accessToken && authResult.idToken) {
        console.log('received authResult');
        window.location.hash = '';
        this.setSession(authResult);

        this.checkAuthenticated();

        console.log('navigating to success route', successRoute);
        this.router.navigateByUrl(successRoute);
      } else if (err) {
        console.log('error parsing hash');
        this.router.navigateByUrl(returnUrl);
        console.error(err);
      } else {
        if (this.checkAuthenticated()) {
          console.log('authenticated -> navigating to success route');
          this.router.navigateByUrl(successRoute);
        } else {
          console.log('not authenticated -> navigating to root');
          if (!returnUrl || returnUrl === '/' || returnUrl.indexOf('welcome-invited') > 0) {
            this.router.navigateByUrl(successRoute);
          } else {
            this.login();
          }
        }
      }

    });
  }

  public getProfile(cb): void {

    if (this.userProfile) {
      cb(null, this.userProfile);
      return;
    }

    const accessToken = this.getAccessToken();
    if (!accessToken) {
      cb('Access Token must exist to fetch profile', null);
    }

    const self = this;
    this.auth0.client.userInfo(accessToken, (err, profile) => {
      if (profile) {
        self.userProfile = profile;
        this.userService.getUser(profile.sub).subscribe(u => {
          profile.nickname = u.name;
          profile.name = u.name;
          cb(null, profile);
        }, e => {
          console.log('AuthService: updating profile', profile);


          // FUCKING DIRY HACK! SORRY SORRY SORRY TO MY FUTURE SELF!
          // FIX SOON PLEASE!!

          // register user
          const request = new CreateOrUpdateUserDTO(
            this.userProfile.sub,
            this.userProfile.nickname,
            this.userProfile.picture);

          this.userService.createOrUpdateUser(request).subscribe(ru => cb(null, profile), ce => cb(ce, profile));
        });
      } else {
        cb(err, profile);
      }
    });
  }

  private setSession(authResult): void {
    console.log('Setting session');
    console.log(authResult);
    // Set the time that the Access Token will expire at
    const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('id_token', authResult.idToken);
    localStorage.setItem('expires_at', expiresAt);
  }

  public checkAuthenticated(): boolean {
    const isAuth = this.isAuthenticated();
    if (isAuth) {

      this.getProfile((error, profile) => {
        if (profile) {
          console.log('got user profile', profile);
          this.currentUserProfile$.next(profile);
        } else {
          if (error) {
            console.error('error retrieving user profile', error);
          } else {
            console.error('no profile and also no error available. this should not happen.');
          }
        }

        this.isLoggedIn$.next(true);
      });

    }

    return isAuth;
  }

  public getAccessToken(): string {
    return localStorage.getItem('access_token');
  }

  getExpiresAt(): any {
    const expiresAt = JSON.parse(localStorage.getItem('expires_at') || '{}');
    return expiresAt;
  }

  public logout(): void {
    // Remove tokens and expiry time from localStorage
    localStorage.removeItem('access_token');
    localStorage.removeItem('id_token');
    localStorage.removeItem('expires_at');

    this.userProfile = null;
    // Go back to the home route
    this.router.navigate(['/']);

    this.isLoggedIn$.next(false);
    this.currentUserProfile$.next(null);
  }

  public isAuthenticated(): boolean {
    // Check whether the current time is past the
    // Access Token's expiry time
    const expiresAt = this.getExpiresAt();
    return new Date().getTime() < expiresAt;
  }

  public renewTokens() {
    this.auth0.checkSession({}, (err, result) => {
      if (err) {
        console.log(err);
      } else {
        this.setSession(result);
        this.scheduleRenewal();
      }
    });
  }

  public scheduleRenewal() {
    if (!this.isAuthenticated()) { return; }
    this.unscheduleRenewal();

    const expiresAt = this.getExpiresAt();

    const expiresIn$ = of(expiresAt).pipe(
      mergeMap(
        x => {
          const now = Date.now();
          // Use timer to track delay until expiration
          // to run the refresh at the proper time
          return timer(Math.max(1, x - now));
        }
      )
    );

    // Once the delay time from above is
    // reached, get a new JWT and schedule
    // additional refreshes
    this.refreshSubscription = expiresIn$.subscribe(
      () => {
        this.renewTokens();
        this.scheduleRenewal();
      }
    );
  }

  public unscheduleRenewal() {
    if (this.refreshSubscription) {
      this.refreshSubscription.unsubscribe();
    }
  }

  resetAuthInformation() {
    localStorage.removeItem(this.idTokenStorageKey);
    localStorage.removeItem(this.tokenStorageKey);
    localStorage.removeItem(this.expiresAtStorageKey);
  }

  handleTokenRefresh(): void {

    if (this.getAccessToken() != null) {
      console.log('AuthService: Handling token refresh');

      // reset access token
      this.resetAuthInformation();

      // raise event
      this.isLoggedIn$.next(false);

      // move to login
      this.login();
    } else {
      console.warn('AuthService: Token expiration handling in progress');
    }
  }
}
