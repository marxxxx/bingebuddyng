import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth.service';

@Injectable()
export class AuthGuard implements CanActivate {

    constructor(public auth: AuthService, public router: Router) { }

    canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
        if (!this.auth.isAuthenticated()) {
            console.warn('AuthGuard: User is not authenticated - directing to login-redirect');
            this.router.navigate(['/callback'], { queryParams: { returnUrl: state.url }});
          return false;
        }
        return true;
    }
}
