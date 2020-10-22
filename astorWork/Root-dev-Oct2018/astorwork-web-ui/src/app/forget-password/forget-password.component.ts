import { Component, OnInit } from '@angular/core';
import { AuthenticationService } from '../shared/authentication.service';
import { UiUtilsService } from '../shared/ui-utils.service';
import { SpinnerDlgComponent } from '../shared/spinner-dlg/spinner-dlg.component';

import { Router, ActivatedRoute } from '@angular/router';
import { CommonLoadingComponent } from '../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-forgetPassword',
  templateUrl: './forget-password.component.html',
  styleUrls: ['./forget-password.component.css'],
  host: {
    '(document:keypress)': 'handleKeyboardEvent($event)'
  }
})
export class ForgetPasswordComponent extends CommonLoadingComponent {

  userName = '';
  email = '';

  constructor(private uiUtilService: UiUtilsService, private authenticationService: AuthenticationService, route: ActivatedRoute, router: Router) {
    super(route, router);
   }

  ngOnInit() {
    super.ngOnInit();
    this.isLoading = false;
  }

  onResetPassword(){
    let data;
    //this.uiUtilService.openDialog(ResetPwDlgComponent, data);
    //this.uiUtilService.openSnackBar("I'm gonna reset yr password!", "Confirm");
  }

  onSubmit() {
    if (this.userName.trim().length > 0 && this.email.trim().length > 0) {
      this.isLoading = true;
      this.authenticationService.resetPassword(this.userName, this.email).subscribe(
        response => {
          if(response == null)
          {
          this.uiUtilService.openSnackBar("Password has been reset and sent to your email " + this.email, "OK");
          this.router.navigateByUrl('/');
          }
          else{
            this.uiUtilService.openSnackBar(response["message"],"OK");
          }
          this.isLoading = false;
        }, error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, "OK");
        }
      );
    }
    else {
      this.uiUtilService.openSnackBar("UserName and Email Required", "OK");
    }
  }
  handleKeyboardEvent(event: KeyboardEvent) {
    if (event.keyCode == 13) {
      this.onSubmit();
    }
  }
}
