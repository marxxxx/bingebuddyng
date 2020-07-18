import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, Subscription, of, timer } from 'rxjs';
import { mergeMap } from 'rxjs/operators';
import * as auth0 from 'auth0-js';

import { CreateOrUpdateUserDTO } from 'src/models/CreateOrUpdateUserDTO';
import { UserService } from 'src/app/@core/services/user.service';
import { UserProfile } from '../../../../models/UserProfile';
import { SettingsService } from '../settings.service';
import { ProfileInfoResult } from './ProfileInfoResult';
import { StateService } from '../state.service';

@Injectable({
  providedIn: 'root',
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
    scope: 'openid profile',
  });

  private isLoggedInSource: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
  isLoggedIn$ = this.isLoggedInSource.asObservable();

  private currentUserProfileSource: BehaviorSubject<UserProfile> = new BehaviorSubject<UserProfile>(null);
  currentUserProfile$ = this.currentUserProfileSource.asObservable();

  constructor(private router: Router, private userService: UserService, private settings: SettingsService, private state: StateService) {
    this.state.pendingFriendRequestsChanged$.subscribe(() => {
      this.onFriendRequestsChanged();
    });
  }

  public login(): void {
    this.auth0.authorize();
  }

  public handleAuthentication(returnUrl?: string): Promise<void> {
    const promise = new Promise<void>((resolve, reject) => {
      console.log('handling authentication', returnUrl);
      const successRoute =
        returnUrl != null && returnUrl !== '/callback' ? returnUrl : this.settings.getIsOnboarded() ? returnUrl : '/onboarding';
      returnUrl = returnUrl || '/';

      this.auth0.parseHash(async (err, authResult) => {
        console.log('parseHash completed', authResult, err);

        if (authResult && authResult.accessToken && authResult.idToken) {
          console.log('received authResult');
          window.location.hash = '';
          this.setSession(authResult);

          await this.checkAuthenticated();

          console.log('navigating to success route', successRoute);
          this.router.navigateByUrl(successRoute);
        } else if (err) {
          console.log('error parsing hash');
          this.router.navigateByUrl(returnUrl);
          console.error(err);
        } else {
          if (await this.checkAuthenticated()) {
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

      resolve();
    });

    return promise;
  }

  public getProfile(): Promise<ProfileInfoResult> {
    const promise = new Promise<ProfileInfoResult>((resolve, reject) => {
      if (this.userProfile) {
        resolve(new ProfileInfoResult(this.userProfile, true));
        return;
      }

      const accessToken = this.getAccessToken();
      if (!accessToken) {
        reject('Access Token must exist to fetch profile');
        return;
      }

      const self = this;
      this.auth0.client.userInfo(accessToken, (err, profile) => {
        if (profile) {
          self.userProfile = profile;
          this.userService.getUser(profile.sub).subscribe(
            (u) => {
              profile.nickname = u.name;
              profile.name = u.name;
              profile.user = u;
              resolve(new ProfileInfoResult(profile, true));
            },
            (e) => {
              console.log('AuthService: user authenticated but not yet registered.', profile, e);
              resolve(new ProfileInfoResult(profile, false));
            }
          );
        } else {
          reject(err);
        }
      });
    });

    return promise;
  }

  private setSession(authResult): void {
    console.log('Setting session');
    console.log(authResult);
    // Set the time that the Access Token will expire at
    const expiresAt = JSON.stringify(authResult.expiresIn * 1000 + new Date().getTime());
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('id_token', authResult.idToken);
    localStorage.setItem('expires_at', expiresAt);
  }

  public checkAuthenticated(): Promise<boolean> {
    const promise = new Promise<boolean>(async (resolve, reject) => {
      const isAuth = this.isAuthenticated();
      if (!isAuth) {
        resolve(false);
        return;
      }

      const result = await this.getProfile();

      if (!result.isRegistered) {
        const user = new CreateOrUpdateUserDTO(
          result.profile.sub,
          result.profile.nickname,
          result.profile.picture,
          null,
          this.settings.getLanguage()
        );
        this.userService.createOrUpdateUser(user).subscribe(
          () => {
            console.log('registered user');
            this.currentUserProfileSource.next(result.profile);
            this.isLoggedInSource.next(true);
            resolve(true);
          },
          (e) => {
            console.error('Failed to register user', e);
            reject(e);
          }
        );
      } else {
        this.currentUserProfileSource.next(result.profile);
        this.isLoggedInSource.next(true);
        resolve(true);
      }
    });

    return promise;
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

    this.isLoggedInSource.next(false);
    this.currentUserProfileSource.next(null);
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
    if (!this.isAuthenticated()) {
      return;
    }
    this.unscheduleRenewal();

    const expiresAt = this.getExpiresAt();

    const expiresIn$ = of(expiresAt).pipe(
      mergeMap((x) => {
        const now = Date.now();
        // Use timer to track delay until expiration
        // to run the refresh at the proper time
        return timer(Math.max(1, x - now));
      })
    );

    // Once the delay time from above is
    // reached, get a new JWT and schedule
    // additional refreshes
    this.refreshSubscription = expiresIn$.subscribe(() => {
      this.renewTokens();
      this.scheduleRenewal();
    });
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
      this.isLoggedInSource.next(false);

      // move to login
      this.login();
    } else {
      console.warn('AuthService: Token expiration handling in progress');
    }
  }

  private onFriendRequestsChanged() {
    this.userService.getUser(this.currentUserProfileSource.value.sub).subscribe((u) => {
      const clonedUser = JSON.parse(JSON.stringify(this.currentUserProfileSource.value));
      clonedUser.user = u;
      this.currentUserProfileSource.next(clonedUser);
    });
  }
}
