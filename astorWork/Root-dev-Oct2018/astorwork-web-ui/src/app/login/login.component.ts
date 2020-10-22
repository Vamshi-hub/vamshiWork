import { Component, OnInit, Renderer2 } from '@angular/core';
import * as moment from 'moment';
import { Router, ActivatedRoute } from '@angular/router';
import { UiUtilsService } from '../shared/ui-utils.service';
import { AuthenticationService } from '../shared/authentication.service';
import { SpinnerDlgComponent } from '../shared/spinner-dlg/spinner-dlg.component';
import { JwtHelper } from '../shared/classes/jwt-helper';
import { AccessToken } from '../shared/classes/access-token';
import { AuthenticationResult } from '../shared/classes/auth-result';
import { CommonLoadingComponent } from '../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  host: {
    '(document:keypress)': 'handleKeyboardEvent($event)'
  }
})
export class LoginComponent extends CommonLoadingComponent {

  userName = '';
  password = '';

  constructor(private uiUtilService: UiUtilsService, private authenticationService: AuthenticationService, route: ActivatedRoute, router: Router, private renderer: Renderer2) {
    super(route, router);
    this.renderer.setStyle(document.body, 'background', 'url(assets/Icons/bg.png) 0% 0% / 100% 100% no-repeat');
  }

  ngOnInit() {
    super.ngOnInit();
    this.isLoading = false;
  }

  onSubmit() {
    if (this.userName.trim().length > 0 && this.password.length > 0) {
      this.isLoading = true;
      this.authenticationService.authenticateUser(this.userName, this.password).subscribe(
        data => {
          if (data.accessToken) {
            try {
              const accessToken = JwtHelper.decodeToken(data.accessToken) as AccessToken;
              window.localStorage.setItem("token_expire_time", moment().add(data.expiresIn, "seconds").format());
              window.localStorage.setItem("access_token_type", data.tokenType);
              window.localStorage.setItem("access_token", data.accessToken);
              window.localStorage.setItem("refresh_token", data.refreshToken);
              window.localStorage.setItem("user_id", String(data.userId));

              this.authenticationService.alertSessionExpire(data.expiresIn * 1000, data.refreshToken, data.userId);
              var wildcard_path = sessionStorage.getItem("wildcard_path");
              console.log("Last path: ", wildcard_path);
              if (wildcard_path) {
                sessionStorage.removeItem('wildcard_path');
                this.router.navigateByUrl(wildcard_path);
              }
              else{
                this.router.navigateByUrl(accessToken.role.defaultPage);
              }
            }
            catch (e) {
              this.isLoading = false;
              this.uiUtilService.openSnackBar(e, "OK");
            }
          }
          else {
            this.isLoading = false;
            this.uiUtilService.openSnackBar(data['message'], "OK");
          }
        }, error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, "OK");
        }
      );
    }
    else {
      this.uiUtilService.openSnackBar("UserName and Password Required", "OK");
    }
  }
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.keyCode == 13) {
      this.onSubmit();
    }
  }
  /*
  alertSessionExpire(timeout: number, accessToken: string) {
    setTimeout(() => {
      if (window.confirm('Your sesson is about to expire, click on "OK" to renew or "Cancel" to log out.')) {
        this.authenticationService.refreshSession(accessToken).subscribe(
          data => {
            try {
              const accessToken = JwtHelper.decodeToken(data.accessToken) as AccessToken;
              window.localStorage.setItem("token_expire_time", moment().add(data.expiresIn, "seconds").format());
              window.localStorage.setItem("access_token_type", data.tokenType);
              window.localStorage.setItem("access_token", data.accessToken);
              if (data.expiresIn > 5 * 60) {
                this.alertSessionExpire((data.expiresIn - 5 * 60) * 1000, data.accessToken);
              }
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
        window.localStorage.setItem("access_token", null);
        this.router.navigateByUrl("/");
      }
    }, timeout);
  }
  */
}
