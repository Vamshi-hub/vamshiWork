import { Component, OnInit } from '@angular/core';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { UserMaster } from '../../shared/classes/user-master';
import { FormBuilder, FormGroup, FormControl, Validators, AbstractControl } from '@angular/forms';
import { ChangePwDlgComponent } from './result-dlg/result-dlg.component';
import { ActivatedRoute, Router, NavigationStart } from '@angular/router';
import * as moment from 'moment';
import { FindValueSubscriber } from 'rxjs/internal/operators/find';
import { AuthenticationService } from '../../shared/authentication.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.css']
})
export class ChangePasswordComponent extends CommonLoadingComponent {
  user: UserMaster;
  theForm: FormGroup;
  errorMsg = false;

  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private authentictionService: AuthenticationService, private materialTrackService: MaterialTrackService, private uiUtilService: UiUtilsService) {
    super(route, router);
   }

  ngOnInit() {
    super.ngOnInit();
    this.theForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', Validators.required],
      confirmPassword: ['', Validators.required]
    }
    );
    this.isLoading = false;
  }

  onSubmit() {
    let requestBody = this.theForm.value;
    requestBody['currentPassword'] = requestBody.currentPassword;
    requestBody['newPassword'] = requestBody.newPassword;
    if (requestBody.newPassword == requestBody.confirmPassword) {
      let data = {};
      let id = window.localStorage.getItem("user_id");
      this.isLoading = true;
      this.materialTrackService.updateUserPassword(parseInt(id), requestBody).subscribe(
        response => {
          console.log(response);
          let data = {};
          if (response == null) {
            data['success'] = true;
            data['message'] = "Password updated successfully";
            data['url'] = "/";
            //this.theForm.reset();
            this.router.navigateByUrl("/");
          }
          else {
            data['success'] = false;
            data['message'] = response['message'];
          }
          this.isLoading = false;
          this.uiUtilService.openSnackBar(data["message"],"OK");
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, 'OK');
        });
    }
    else {
      console.log("error");
      this.errorMsg = true;
      console.log(this.errorMsg);
    }
  }

}

export class PasswordValidation {

  static MatchPassword(AC: AbstractControl) {
    let password = AC.get('newPassword').value; // to get value in input tag
    let confirmPassword = AC.get('confirmPassword').value; // to get value in input tag
    if (password != confirmPassword) {
      console.log('false');
      AC.get('confirmPassword').setErrors({ MatchPassword: true })
    } else {
      console.log('true');
      AC.get('confirmPassword').setErrors(null);
    }
  }
}

