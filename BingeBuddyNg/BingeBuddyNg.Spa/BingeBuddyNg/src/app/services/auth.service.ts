import { Injectable, EventEmitter } from '@angular/core';
import * as auth0 from 'auth0-js';
import { Router } from '@angular/router';
import { UserProfile } from '../../models/UserProfile';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private userProfile: UserProfile;

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

  constructor(public router: Router) { }

  public login(): void {
    this.auth0.authorize();
  }

  public handleAuthentication(returnUrl?: string): void {

    const successRoute = returnUrl != null ? returnUrl : '/activity-feed';

    this.auth0.parseHash((err, authResult) => {
      if (authResult && authResult.accessToken && authResult.idToken) {
        window.location.hash = '';
        this.setSession(authResult);

        if (this.isAuthenticated()) {
          this.isLoggedIn$.next(true);
        }

        this.router.navigate([successRoute]);
      } else if (err) {
        this.router.navigate(['/']);
        console.log(err);
      } else {
        this.checkAuthenticated();

        console.log(this.getAccessToken());
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
      throw new Error('Access Token must exist to fetch profile');
    }

    const self = this;
    this.auth0.client.userInfo(accessToken, (err, profile) => {
      if (profile) {
        self.userProfile = profile;
      }
      cb(err, profile);
    });
  }

  private setSession(authResult): void {
    // Set the time that the Access Token will expire at
    const expiresAt = JSON.stringify((authResult.expiresIn * 1000) + new Date().getTime());
    localStorage.setItem('access_token', authResult.accessToken);
    localStorage.setItem('id_token', authResult.idToken);
    localStorage.setItem('expires_at', expiresAt);
  }

  public checkAuthenticated(): boolean {
    const isAuth = this.isAuthenticated();
    if (isAuth) {
      console.log('user is authenticated');
      this.isLoggedIn$.next(true);
    }

    return isAuth;
  }

  public getAccessToken(): string {
    return localStorage.getItem('access_token');
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
  }

  public isAuthenticated(): boolean {
    // Check whether the current time is past the
    // Access Token's expiry time
    const expiresAt = JSON.parse(localStorage.getItem('expires_at') || '{}');
    return new Date().getTime() < expiresAt;
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
