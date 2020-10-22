import { Component, OnInit, NgModule } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Router, ActivatedRoute, ParamMap } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { VendorMaster } from '../../material-track/classes/vendor-master';
import { ConfigurationService } from '../configuration.service';
import { UserMaster } from '../../shared/classes/user-master';
import { LocationMaster } from '../../shared/classes/location-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-vendor-details',
  templateUrl: './vendor-details.component.html',
  styleUrls: ['./vendor-details.component.css']
})
export class VendorDetailsComponent extends CommonLoadingComponent {
  theForm: FormGroup;
  submit = "Save";
  contactPeople: UserMaster[];
  locations: LocationMaster[];
  vendorDetails: VendorMaster;
  editing = false;
  id: number;
  errorMsg = "";
  profile = "Create Vendor"
  haveContactPerson = false;
  haveLocation = false;
  editSelf = false;

  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private uiUtilService: UiUtilsService, private configurationService: ConfigurationService) {
    super(route, router);
   }

  ngOnInit() {
    super.ngOnInit();

    this.route.url.subscribe(urlSegments => {
      const userId = window.localStorage.getItem('vendor_id');
      if (urlSegments[urlSegments.length - 1].path == userId)
        this.editSelf = true;
    })

    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      if (this.id != 0) {
        this.getVendorDetailsById(this.id);
      }
      else {
        this.isLoading = false;;
      }
    });
    
    this.createForm();
  }

  createForm() {
    this.theForm = this.fb.group({
      vendorName: [{ value: '' }, Validators.required],
      cycleDays: new FormControl("", [Validators.required, Validators.min(1)]),
    });
  }

  OnCancel(){
    this.router.navigateByUrl('/configuration/vendor-master');
  }
  onSubmit() {
    let requestBody = this.theForm.value;
    let vendor: VendorMaster;
    vendor = new VendorMaster();
    vendor.name = requestBody['vendorName'];
    vendor.cycleDays = requestBody['cycleDays'];
    
    this.isLoading = true;
    (this.id == 0 ? this.configurationService.createVendor(vendor) : this.configurationService.updateVendor(this.id, vendor)).subscribe(
      response => {
        let data = {};
        if (response == null || !response["message"]) {
          data['success'] = true;
          data['message'] = " ";
          if (this.id == 0)
            data['message'] = "Vendor Created Successfully";
          else
            data['message'] = "Vendor profile Updated Successfully.";

          this.uiUtilService.openSnackBar(data['message'], "OK");
          this.router.navigateByUrl('/configuration/vendor-master');
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

  getVendorDetailsById(vendorId: number) {
    this.profile = "Edit Vendor";
    this.submit = "Update";
    this.configurationService.getVendorDetails(vendorId).subscribe(
      data => {
        this.vendorDetails = data;
        this.theForm.clearValidators();
        this.theForm.controls["vendorName"].setValue(this.vendorDetails.name);
        this.theForm.controls["cycleDays"].setValue(this.vendorDetails.cycleDays);
        this.contactPeople = this.vendorDetails.contactPeople;
        
        this.locations = this.vendorDetails.locations;

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
