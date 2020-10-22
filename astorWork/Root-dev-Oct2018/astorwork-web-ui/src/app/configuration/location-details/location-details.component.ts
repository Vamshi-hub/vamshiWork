import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { ConfigurationService } from '../configuration.service';
import { LocationMaster } from '../../material-track/classes/location-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { SiteMaster } from '../classes/site-master';
import { isNgTemplate } from '@angular/compiler';

@Component({
  selector: 'app-location-details',
  templateUrl: './location-details.component.html',
  styleUrls: ['./location-details.component.css']
})
export class LocationDetailsComponent extends CommonLoadingComponent {
  theForm: FormGroup;
  title = 'Location Details';
  allSites: SiteMaster[];
  Sites: SiteMaster[];
  id = 0;
  showVendorSelection = false;
  disabled = false;
  submit = "Save";

  constructor(private fb: FormBuilder, route: ActivatedRoute, router: Router,
    private materialTrackService: MaterialTrackService, private uiUtilService: UiUtilsService,
    private configurationService: ConfigurationService) {
    super(route, router);
    this.createForm();
  }

  ngOnInit() {
    super.ngOnInit();
    this.getSites();
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      if (this.id > 0)
          this.title = 'Edit Location';
      else
        this.title = 'Create Location';
    });
  }

  onCancel() {
    this.router.navigate(['configuration', 'location-master']);
  }

  createForm() {
    this.theForm = this.fb.group({
      name: ['', Validators.required],
      description: '',
      siteId: ['', Validators.required]
    });
  }

  toggle() {
    if (this.showVendorSelection) {

      this.allSites = this.Sites.filter(s => s.vendorId != 0);
    }
    else {
      this.allSites = this.Sites.filter(s => s.vendorId == 0);
    }
    this.theForm.controls['siteId'].setValue('');
  }

  onSubmit() {
    this.isLoading = true;
    let location = this.theForm.value as LocationMaster;
    location.type = this.showVendorSelection ? 0 : -1;
    if (this.id) {
      location.id = this.id;
      this.configurationService.editLocation(location).subscribe(data => {
        this.isLoading = false;
        this.router.navigate(['configuration', 'location-master']);
        this.uiUtilService.openSnackBar("Edit location successfully", "OK");
      }, error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      });
    }
    else {
      this.configurationService.createLocation(location).subscribe(data => {
        this.isLoading = false;
        this.router.navigate(['configuration', 'location-master']);
        this.uiUtilService.openSnackBar("Create location successfully", "OK");
      }, error => {
        this.isLoading = false;;
        this.uiUtilService.openSnackBar(error, "OK");
      });
    }
  }

  getSites() {
    this.configurationService.getSites().subscribe(
      data => {
        this.Sites = data;
        this.allSites = this.Sites;
        if (this.id > 0) {
          this.getLocationDetails();
          if (this.accessRight < 2)
            this.title = 'Location Details';
        }
        else {
          this.allSites = this.Sites.filter(s => s.vendorId == 0);
          this.isLoading = false;
        }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getLocationDetails() {
    this.submit = "Update";
    this.configurationService.getLocationDetais(this.id).subscribe(
      data => {
        this.theForm.setValue({
          name: data.name,
          description: data.description,
          siteId: data.siteId
        });
        this.showVendorSelection = data.type == 0 ? true : false;
        if (this.showVendorSelection) {
          this.allSites = this.Sites.filter(s => s.vendorId != 0);
        }
        else
          this.allSites = this.Sites.filter(s => s.vendorId == 0);
        this.disabled = true;
        this.isLoading = false;

      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }
}
