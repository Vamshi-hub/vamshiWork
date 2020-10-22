import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { tap, catchError } from 'rxjs/operators'
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';

import { environment } from '../../environments/environment';

import { AuthenticationResult } from './classes/auth-result';
import { UiUtilsService } from './ui-utils.service';
import { JwtHelper } from './classes/jwt-helper';
import * as moment from 'moment';
import { Router } from '@angular/router';
import { AccessToken } from './classes/access-token';

@Injectable()
export class AuthenticationService {
  refreshTimer: any;

  constructor(private http: HttpClient, private uiUtilService: UiUtilsService, private router: Router) { }

  private handleError(error: HttpErrorResponse) {
    console.log('HTTP error', error);

    let message = "Something bad happened; please try again later.";

    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      message = 'An error occurred:', error.error.message;
    } if (error.status == 401) {
      message = 'The user name and password combination is invalid';
    }
    // return an ErrorObservable with a user-facing error message
    return throwError(message);
  };

  authenticateUser(userName: string, password: string) {
    const endpoint = `${environment.api_base_url}/authentication/login`;
    return this.http.post<AuthenticationResult>(endpoint, {
      userName: userName,
      password: password
    }).pipe(
      catchError(this.handleError)
    );
  }

  resetPassword(userName: string, email: string) {
    const endpoint = `${environment.api_base_url}/authentication/forgetPassword`;
    return this.http.post<AuthenticationResult>(endpoint, {
      userName: userName,
      email: email
    }).pipe(
      catchError(this.handleError)
    );
  }


  refreshSession(refreshToken: string, userID: number) {
    const endpoint = `${environment.api_base_url}/authentication/refresh`;
    return this.http.post<AuthenticationResult>(endpoint, {
      refreshToken: refreshToken,
      userId: userID
    }).pipe(
      catchError(this.handleError)
    );
  }

  alertSessionExpire(timeout: number, refreshToken: string, userID: number, suppressAleart = false) {
    this.refreshTimer = setTimeout(() => {
      if (window.confirm('Your session is about to expire, click on "OK" to renew or "Cancel" to log out.') || suppressAleart) {
        this.refreshSession(refreshToken, userID).subscribe(
          data => {
            try {
              window.localStorage.setItem("token_expire_time", moment().add(data.expiresIn, "seconds").format());
              window.localStorage.setItem("access_token_type", data.tokenType);
              window.localStorage.setItem("access_token", data.accessToken);
              window.localStorage.setItem("refresh_token", data.refreshToken);
              window.localStorage.setItem("user_id", String(data.userId));

              this.alertSessionExpire(data.expiresIn * 1000, data.refreshToken, data.userId, suppressAleart);
            }
            catch (e) {
              this.uiUtilService.openSnackBar(e, "OK");
            }
            this.uiUtilService.closeAllDialog();
          }, error => {
            this.uiUtilService.closeAllDialog();
            this.uiUtilService.openSnackBar("error", "OK");
          }
        )
      }
      else {
        window.localStorage.removeItem("access_token");
        this.router.navigateByUrl("/");
      }
    }, timeout);
  }

  logOut() {
    if (this.refreshTimer) {
      clearTimeout(this.refreshTimer);
    }
    window.localStorage.removeItem('access_token');
    window.localStorage.removeItem('refresh_token');
    window.localStorage.removeItem('user_id');
    window.localStorage.removeItem('project_id');
    window.sessionStorage.clear();
    this.router.navigateByUrl('/');
  }

}
