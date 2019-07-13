
import { tap } from 'rxjs/operators';
import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpEvent, HttpRequest, HttpHeaders, HttpResponse, HttpErrorResponse } from '@angular/common/http';



@Injectable()
export class AuthHttpInterceptor implements HttpInterceptor {

    //////////////////////////////////////////////////////////////////////////////
    // construction
    //////////////////////////////////////////////////////////////////////////////
    constructor(private authService: AuthService) {
    }

    //////////////////////////////////////////////////////////////////////////////
    // functions
    //////////////////////////////////////////////////////////////////////////////
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {

        let changedRequest = request;

        if (this.isApiCall(request)) {
            changedRequest = this.addAccessTokenToRequest(request);
        }

        return next.handle(changedRequest).pipe(tap((event: HttpEvent<any>) => {
            if (event instanceof HttpResponse) {
                // do stuff with response if you want
            }
        }, (err: any) => {
            if (err instanceof HttpErrorResponse) {
                if (err.status === 401) {
                    // redirect to the login route
                    console.warn('AuthHttpInterceptor: request is unauthorized - triggering token refresh!');
                    this.authService.handleTokenRefresh();
                }
            }
        }));
    }


    isApiCall(request: HttpRequest<any>): boolean {
        return request.url.indexOf('/api') >= 0;
    }

    addAccessTokenToRequest(request: HttpRequest<any>): HttpRequest<any> {
        const token = this.authService.getAccessToken();

        console.log(`AuthHttpInterceptor: authenticating request to ${request.url}`);

        // HttpHeader object immutable - copy values
        request = request.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });

        return request;
    }

}
