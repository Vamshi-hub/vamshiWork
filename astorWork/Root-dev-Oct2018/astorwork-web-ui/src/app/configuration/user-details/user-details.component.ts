import { Component, OnInit, NgModule } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Role } from '../../shared/classes/role';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { UserDetails } from '../../material-track/classes/user-details';
import { ProjectMaster } from '../../material-track/classes/project-master';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { Users } from '../../material-track/classes/users';
import { normalCharacterValidator } from '../../shared/directives/normal-char.directive';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { SiteMaster } from '../classes/site-master';
import { ConfigurationService } from '../configuration.service';
import { VendorMaster } from '../../material-track/classes/vendor-master';

@Component({
  selector: 'app-user-details',
  templateUrl: './user-details.component.html',
  styleUrls: ['./user-details.component.css']
})
export class UserDetailsComponent extends CommonLoadingComponent {
  theForm: FormGroup;
  submit = "Save";
  allroles: Role[];
  allSites: SiteMaster[];
  allProjects: ProjectMaster[];
  allVendors: VendorMaster[];
  sites: SiteMaster[];

  userDetails: UserDetails;
  checked: boolean;
  selectedRole: number;
  selectedSite: number;
  editing = false;
  id: number;
  errorMsg = "";
  title = 'User Details';
  showSiteSelection = false;
  showVendorSelection = false;
  showProjectSelection = false;
  editSelf = false;

  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService
    , private configurationService: ConfigurationService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.url.subscribe(urlSegments => {
      const userId = window.localStorage.getItem('user_id');
      if (urlSegments[urlSegments.length - 1].path == userId)
        this.editSelf = true;
    })
    this.selectedRole = 0;
    this.selectedSite = 0;
    this.checked = true;
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      if (this.id > 0)
        this.title = 'Edit User';
      else
        this.title = 'Create User';
    });
    this.createForm();
    this.getRoles();
    this.getProjects();
    this.getSites();
    this.getVenodrs();
  }

  createForm() {
    this.theForm = this.fb.group({
      userName: ['', [Validators.required, normalCharacterValidator]],
      personName: ['', Validators.required],
      email: new FormControl('', [Validators.required, Validators.email]),
      role: ['', Validators.required],
      site: ['', Validators.required],
      project: ['', Validators.required],
      vendor: ['', Validators.required]
    });

    this.theForm.get('role').valueChanges.subscribe(val => {
      if (this.allroles) {
        let role = this.allroles.find(r => r.id == val);

        this.theForm.controls['site'].clearValidators();
        this.theForm.controls['site'].setValue('');
        this.theForm.controls['project'].clearValidators();
        this.theForm.controls['project'].setValue('');
        this.theForm.controls['vendor'].clearValidators();
        this.theForm.controls['vendor'].setValue('');
        this.showSiteSelection = false;
        this.showProjectSelection = false;
        this.showVendorSelection = false;

        if (role.id == 4 || role.id == 5) {
          this.showProjectSelection = true;
          this.theForm.controls['project'].setValidators(Validators.required);
          this.theForm.controls['project'].setValue('');
        }
        if (role.id == 5 || role.id == 8 || role.id == 7) {
          this.showSiteSelection = true;
          this.allSites = role.id == 5 ? this.sites.filter(s => s.vendorId == 0) : this.sites.filter(s => s.vendorId != 0);
          this.theForm.controls['site'].setValidators(Validators.required);
          this.theForm.controls['site'].setValue('');
        }
        // if (role.id == 7) {
        //   this.showVendorSelection = true;
        //   this.theForm.controls['vendor'].setValidators(Validators.required);
        //   this.theForm.controls['vendor'].setValue('');
        // }
      }
    });
  }

  OnCancel() {
    this.router.navigateByUrl('/configuration/user-master');
  }

  onSubmit() {
    let requestBody = this.theForm.value;
    let user: Users;
    user = new Users();
    user.userID = this.id;
    user.userName = requestBody['userName'];
    user.personName = requestBody['personName'];
    user.email = requestBody['email'];
    user.roleID = requestBody['role'];
    user.siteID = requestBody['site'];
    user.projectID = requestBody['project'];
    user.vendorID = requestBody['vendor'];
    if (user.roleID != 5 && user.roleID != 8 && user.roleID != 7)
      user.siteID = 0;
    if (user.roleID != 4 && user.roleID != 5)
      user.projectID = 0;
    // if (user.roleID != 7)
       user.vendorID = 0;
    user.role = '';
    user.site = '';
    user.isActive = this.checked;
    this.isLoading = true;
    (this.id == 0 ? this.materialTrackService.createUser(user) : this.materialTrackService.updateUser(this.id, user)).subscribe(
      response => {
        let data = {};
        if (response == null) {
          data['success'] = true;
          data['url'] = "/material-tracking/userprofile/" + user.userID;
          data['message'] = " ";
          if (this.id == 0)
            data['message'] = "User Created Successfully. Password has sent to " + user.email;
          else
            data['message'] = "User profile Updated Successfully. ";

          this.uiUtilService.openSnackBar(data['message'], "OK");
          this.router.navigateByUrl('/configuration/user-master');
          //this.uiUtilService.openDialog(ChangePwDlgComponent, data);
        }
        else {
          data['success'] = false;
          data['message'] = response['message'];
          this.errorMsg = data["message"];

        }
        this.isLoading = false;
      },
      error => {
        this.errorMsg = error["message"];
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });


  }

  getUserDetailsById(userId: number) {
    this.submit = "Update";
    this.materialTrackService.getUserDetails(userId).subscribe(
      data => {
        this.userDetails = data;
        this.theForm.clearValidators();
        this.selectedRole = this.userDetails.roleID;
        this.selectedSite = this.userDetails.siteID;
        this.theForm.controls["role"].setValue(this.selectedRole);
        this.theForm.controls["site"].setValue(this.selectedSite);
        this.theForm.controls["project"].setValue(this.userDetails.projectID)
        this.theForm.controls["vendor"].setValue(this.userDetails.vendorID)
        this.theForm.controls["userName"].setValue(this.userDetails.userName);
        this.theForm.controls["personName"].setValue(this.userDetails.personName);
        this.theForm.controls["email"].setValue(this.userDetails.email);
        if (userId != 0) {
          this.checked = this.userDetails.isActive;
          this.editing = true;
          this.theForm.controls['userName'].disable();

        }
        else {
          this.checked = true;
        }
        if (data.roleID == 5 || data.roleID == 8 || data.roleID == 7) {
          this.showSiteSelection = true;
          this.allSites = data.roleID == 5 ? this.sites.filter(s => s.vendorId == 0) : this.sites.filter(s => s.vendorId != 0);
          this.theForm.controls["site"].setValidators(Validators.required);
        }
        else {
          this.showSiteSelection = false;
        }
        if (this.accessRight < 2)
          this.title = 'User Details';
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getRoles() {
    this.materialTrackService.getRoles().subscribe(
      data => {
        this.allroles = data;
        if (this.id != 0) {
          this.getUserDetailsById(this.id);
        }
        else {
          this.isLoading = false;
        }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getSites() {
    this.configurationService.getSites().subscribe(
      data => {
        this.sites = data;
        this.allSites = this.sites;

        // if (this.id != 0) {
        //   this.getUserDetailsById(this.id);
        // }
        // else {
        //   this.isLoading = false;
        // }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getProjects() {
    this.materialTrackService.getListProjects().subscribe(
      data => {
        this.allProjects = data;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getVenodrs() {
    this.materialTrackService.getListVendors().subscribe(
      data => {
        this.allVendors = data;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  get userName() { return this.theForm.get('userName'); }
}
