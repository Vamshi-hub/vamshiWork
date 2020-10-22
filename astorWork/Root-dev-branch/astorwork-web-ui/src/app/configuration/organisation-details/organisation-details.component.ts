import { Component, OnInit, NgModule } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { OrganisationMaster } from '../../material-track/classes/organisation-master';
import { ConfigurationService } from '../configuration.service';
import { UserMaster } from '../../shared/classes/user-master';
import { LocationMaster } from '../../shared/classes/location-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { MatSelectChange } from '@angular/material';
import { OrganisationType } from '../../shared/classes/enums';

@Component({
  selector: 'app-organisation-details',
  templateUrl: './organisation-details.component.html',
  styleUrls: ['./organisation-details.component.css']
})
export class OrganisationDetailsComponent extends CommonLoadingComponent {
  theForm: FormGroup;
  submit = "Save";
  contactPeople: UserMaster[];
  locations: LocationMaster[];
  organisationDetails: OrganisationMaster;
  editing = false;
  id: number;
  errorMsg = "";
  profile = "Create Organisation"
  haveContactPerson = false;
  haveLocation = false;
  editSelf = false;
  isNotVendor =true;
  disable = false;
  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private uiUtilService: UiUtilsService, private configurationService: ConfigurationService) {
    super(route, router);
   }

  ngOnInit() {
    super.ngOnInit();

    this.route.url.subscribe(urlSegments => {
      const userId = window.localStorage.getItem('organisation_id');
      if (urlSegments[urlSegments.length - 1].path == userId)
        this.editSelf = true;
    })

    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      if (this.id != 0) {
        this.getOrganisationDetailsById(this.id);
      }
      else {
        this.isLoading = false;;
      }
    });
    
    this.createForm();
  }

  createForm() {
    this.theForm = this.fb.group({
      organisationName: ['', Validators.required],
      organisationType: ['',Validators.required],
      cycleDays:!this.isNotVendor ?['', [Validators.required, Validators.min(1)]]: [''],
    });
  }
  organisationTypeChange(event: MatSelectChange){
    if(event.value == OrganisationType.Vendor){
      this.isNotVendor =false;
      this.theForm.controls['cycleDays'].setValue('');
      this.theForm.controls['cycleDays'].setValidators([Validators.required, Validators.min(1)]);
    }
    else{
      this.isNotVendor = true;
      this.theForm.controls['cycleDays'].clearValidators();
    }

    
  }
  OnCancel(){
    this.router.navigateByUrl('/material-tracking/organisation-master');
  }
  onSubmit() {
    let requestBody = this.theForm.value;
    let organisation: OrganisationMaster;
    organisation = new OrganisationMaster();
    organisation.name = requestBody['organisationName'];
    organisation.organisationType = requestBody['organisationType'];
    if(organisation.organisationType == OrganisationType.Vendor)
      organisation.cycleDays = requestBody['cycleDays'];
      else
      organisation.cycleDays = 1;
    
    this.isLoading = true;
    (this.id == 0 ? this.configurationService.createOrganisation(organisation) : this.configurationService.updateOrganisation(this.id, organisation)).subscribe(
      response => {
        let data = {};
        if (response == null || !response["message"]) {
          data['success'] = true;
          data['message'] = " ";
          if (this.id == 0)
            data['message'] = "Organisation Created Successfully";
          else
            data['message'] = "Organisation  Updated Successfully.";

          this.uiUtilService.openSnackBar(data['message'], "OK");
          this.router.navigateByUrl('/material-tracking/organisation-master');
        }
        else {
          data['success'] = false;
          data['message'] = response['message'];
          this.errorMsg = data["message"];
          this.uiUtilService.openSnackBar(data['message'], "OK");
        }
        this.isLoading = false;
      },
      error => {
        this.errorMsg = error["message"];
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  getOrganisationDetailsById(organisationId: number) {
    this.profile = "Edit Organisation";
    this.submit = "Update";
    this.disable =true;
    this.configurationService.getOrganisationDetails(organisationId).subscribe(
      data => {
        this.organisationDetails = data;
        this.theForm.clearValidators();
        this.theForm.controls["organisationName"].setValue(this.organisationDetails.name);
        this.theForm.controls["organisationType"].setValue(this.organisationDetails.organisationType.toString());
        if(this.organisationDetails.organisationType == OrganisationType.Vendor){
        this.theForm.controls["cycleDays"].setValue(this.organisationDetails.cycleDays);
        this.isNotVendor=false;
        }
        else
        this.isNotVendor = true;
        this.contactPeople = this.organisationDetails.contactPeople;
        
        this.locations = this.organisationDetails.locations;

        if (this.contactPeople.length > 0)
          this.haveContactPerson = true;
        if (this.locations.length > 0)
          this.haveLocation = true;

          this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }
}
