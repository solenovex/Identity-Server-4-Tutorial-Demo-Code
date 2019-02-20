import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { OpenIdConnectService } from './open-id-connect.service';
import { Observable } from 'rxjs';

@Injectable()
export class AuthorizationHeaderInterceptor implements HttpInterceptor {
  constructor(private oidc: OpenIdConnectService) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (this.oidc.userAvailable) {
      request = request.clone({
        setHeaders: {
          Authorization: `${this.oidc.user.token_type} ${this.oidc.user.access_token}`
        }
      });
    }

    return next.handle(request);
  }
}
