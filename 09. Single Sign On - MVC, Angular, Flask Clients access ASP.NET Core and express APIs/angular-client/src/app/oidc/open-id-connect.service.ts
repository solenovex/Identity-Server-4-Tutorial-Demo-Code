import { Injectable } from '@angular/core';
import { UserManager, User } from 'oidc-client';
import { environment } from 'src/environments/environment';
import { ReplaySubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class OpenIdConnectService {
  private userManager = new UserManager(environment.openIdConnectSettings);

  private currentUser: User;

  get userAvailable(): boolean {
    return !!this.currentUser;
  }

  get user(): User {
    return this.currentUser;
  }

  userLoaded$ = new ReplaySubject<boolean>(1);

  constructor() {
    this.userManager.clearStaleState();

    this.userManager
      .getUser()
      .then(user => {
        if (user) {
          this.currentUser = user;
          this.userLoaded$.next(true);
        } else {
          this.currentUser = null;
          this.userLoaded$.next(false);
        }
      })
      .catch(err => {
        this.currentUser = null;
        this.userLoaded$.next(false);
      });

    this.userManager.events.addUserLoaded(user => {
      console.log('user loaded:', user);
      this.currentUser = user;
      this.userLoaded$.next(true);
    });

    this.userManager.events.addUserUnloaded(user => {
      console.log('user unloaded');
      this.currentUser = null;
      this.userLoaded$.next(false);
    });
  }

  triggerSignIn() {
    this.userManager.signinRedirect().then(() => {
      console.log('triggerSignIn');
    });
  }

  handleCallback() {
    this.userManager.signinRedirectCallback().then(user => {
      this.currentUser = user;
      console.log('handleCallback');
    });
  }

  handleSilentCallback() {
    this.userManager.signinSilentCallback().then(user => {
      this.currentUser = user;
      console.log('handleSilentCallback');
    });
  }

  triggerSignOut() {
    this.userManager.signoutRedirect().then(res => {
      console.log('triggerSignOut');
    });
  }
}
