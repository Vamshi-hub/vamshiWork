import { Component, OnInit } from '@angular/core';

import { AuthenticationService } from './shared/authentication.service';
import * as moment from 'moment';
import { Router, ActivatedRoute, NavigationStart } from '@angular/router';
import { AccessToken } from './shared/classes/access-token';
import { JwtHelper } from './shared/classes/jwt-helper';

import { filter, first } from 'rxjs/operators';
import { UiUtilsService } from './shared/ui-utils.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})

export class AppComponent {
  title = 'astorWork';
  show_bg = true;
  this_year = new Date().getFullYear();

  constructor(private route: ActivatedRoute, private router: Router,
    private authenticationService: AuthenticationService, private uiUtilService: UiUtilsService) { }

  ngOnInit() {
    this.checkSession();
  }

  checkSession() {
    var logout = true;
    this.show_bg = true;
    var accessTokenStr = window.localStorage.getItem("access_token");
    if (accessTokenStr) {
      var refreshTokenStr = window.localStorage.getItem("refresh_token");
      if (refreshTokenStr) {
        var userIdStr = window.localStorage.getItem("user_id");
        var tokenExpireStr = window.localStorage.getItem("token_expire_time");
        if (tokenExpireStr) {
          var tokenExpireTime = moment(tokenExpireStr);
          var now = moment();
          if (tokenExpireTime >= now) {
            logout = false;
            this.show_bg = false;

            const accessToken = JwtHelper.decodeToken(accessTokenStr) as AccessToken;

            var duration = moment.duration(tokenExpireTime.diff(now));
            var ms = duration.asMilliseconds();

            let suppress_alert = false;
            if(window.sessionStorage.getItem("suppress_alert"))
              suppress_alert = true;
            this.authenticationService.alertSessionExpire(ms, refreshTokenStr, +userIdStr, suppress_alert);

            //this.router.navigateByUrl(accessToken.role.defaultPage);
          }
          else
            console.log('Token expired: ' + tokenExpireTime);
        }
        else
          console.log('Token expire time invalid');
      }
      else
        console.log('Refresh Token invalid');
    }
    else
      console.log('Access Token invalid');

    if (logout) {
      if(window.location.pathname != '/' && window.location.pathname != '/login')
            sessionStorage.setItem("wildcard_path", window.location.pathname);
            
      this.router.navigateByUrl("/");
    }
  }
}