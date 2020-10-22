import { Component, OnInit, ViewChild, HostListener, ElementRef } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { ParamMap, ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { Page } from '../../shared/classes/page';
import { Module } from '../../shared/classes/module';
import { RoleDetails } from '../../shared/classes/role-details';
import { ModulePage } from '../../shared/classes/module-page';
import { MatAccordion } from '@angular/material';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-role-details',
  templateUrl: './role-details.component.html',
  styleUrls: ['./role-details.component.css'] 
})
export class RoleDetailsComponent extends CommonLoadingComponent {
  @ViewChild(MatAccordion) accordion: MatAccordion;

  @HostListener('document:click', ['$event'])
  clickout(event) {
    if(event.path.find(x => x.nodeName == 'MAT-EXPANSION-PANEL')== undefined) {
      this.accordion.closeAll();
    }
  }

  theForm: FormGroup;
  allDefaultPages: Page[];
  modules: Module[];
  roleDetails: RoleDetails;
  checked: boolean;
  editing = true;
  id: number;
  errorMsg = "";
  title = "Create Role";
  submit = "Save";

  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
   }
  ngOnInit() {
    super.ngOnInit();
    
    this.checked = true;
    this.getDefaultPages();
    this.createForm();
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      this.getModules();
    });
  }

  createForm() {
    this.theForm = this.fb.group({
      roleName: ['', Validators.required],
      defaultpage: ['', Validators.required],
      accesslevel: ["0", Validators.required],
    });
    this.theForm.get('defaultpage').valueChanges.subscribe(val => {
      for (let module in this.modules) {
        for (let page in this.modules[module]["listofpages"]) {
          if(this.modules[module]["listofpages"][page].pageId == val)
          this.modules[module]["listofpages"][page]["accessLevel"] = "1";
        }
      }
    });
    this.theForm.get('accesslevel').valueChanges.subscribe(val => {
      for (let module in this.modules) {
        for (let page in this.modules[module]["listofpages"]) {
          this.modules[module]["listofpages"][page]["accessLevel"] = val;
        }
      }
    });
  }

  OnReset()
  {
    this.isLoading=true;
    this.createForm();
    this.getModules();
  }

  OnCancel(){
    this.router.navigateByUrl('/configuration/role-master');
  }
  
  onSubmit() {
    let requestBody = this.theForm.value;
    let roleDetails: RoleDetails;
    roleDetails = new RoleDetails();
    roleDetails.roleId=this.id;
    roleDetails.roleName=requestBody["roleName"];
    roleDetails.defaultPageId=requestBody["defaultpage"];
    roleDetails.listofPages=Array<ModulePage>();
    console.log(this.modules);
    for (let mIndex in this.modules) {
      var mModules=this.modules[mIndex];
      for (let mpIndex in this.modules[mIndex].listofpages) {
        var mPage = this.modules[mIndex].listofpages[mpIndex];
        let modulePage: ModulePage;
        modulePage= new ModulePage();
        modulePage.pageId=mPage.pageId;
        modulePage.pageName=mPage.pageName;
        modulePage.accessLevel=mPage.accessLevel;
        modulePage.moduleId=mModules.moduleId;
        modulePage.moduleName=mModules.moduleName;
        roleDetails.listofPages.push(modulePage);
      }
    }
    this.isLoading = true;
    (this.id == 0 ? this.materialTrackService.createRole(roleDetails) : this.materialTrackService.updateRole(this.id, roleDetails)).subscribe(
      response => {
        let data = {};
        if (response == null) {
          data['success'] = true;
          data['message'] = " ";
          if (this.id == 0)
            data['message'] = "Role Created Successfully.";
          else
            data['message'] = "Role Updated Successfully.";
          this.uiUtilService.openSnackBar(data['message'], "OK");
          this.router.navigateByUrl('/configuration/role-master');
        }
        else {
          data['success'] = false;
          data['message'] = response['message'];
          this.errorMsg = data["message"];
          this.uiUtilService.openSnackBar(this.errorMsg, "OK");
        }
        this.isLoading = false;
      },
      error =>{
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    )
  }

  getRoleDetailsById(roleId: number) {
    this.title = "Edit Role";
    this.submit = "Update";
    this.materialTrackService.getRoleDetails(roleId).subscribe(
      data => {
        this.roleDetails = data[0];
        this.theForm.controls["roleName"].setValue(this.roleDetails.roleName);
        this.theForm.controls["defaultpage"].setValue(this.roleDetails.defaultPageId);
        for (let rmpIndex in this.roleDetails.listofPages) {
          var rmPage = this.roleDetails.listofPages[rmpIndex];
          for (let mIndex in this.modules) {
            for (let mpIndex in this.modules[mIndex].listofpages) {
              var mPage = this.modules[mIndex].listofpages[mpIndex];
              if (mPage.pageId == rmPage.pageId) {
                mPage.accessLevel = rmPage.accessLevel.toString();
                break;
              }
            }
          }
        }
        if (roleId != 0)
          this.editing = false;
        else
          this.checked = true;
          this.isLoading = false;
      },
      error => {
        console.log(error);
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getDefaultPages() {
    this.materialTrackService.getDefaultPages().subscribe(
      data => {
        this.allDefaultPages = data;
      },
      error => {
        this.uiUtilService.openSnackBar("Dont have any default Page", "OK");
      }
    )
  }

  getModules() {
    this.materialTrackService.getModules().subscribe(
      data => {
        this.modules = data;
        if (this.id != 0)
          this.getRoleDetailsById(this.id);
        else
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar("No Modules", "OK");
      }
    )
  }

}
