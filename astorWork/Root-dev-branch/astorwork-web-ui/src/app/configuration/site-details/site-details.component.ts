import { Component, OnInit } from '@angular/core';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { OrganisationMaster } from '../../material-track/classes/organisation-master';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { ConfigurationService } from '../configuration.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { SiteMaster } from '../classes/site-master';
import { Country } from '../classes/country';
import { MatSelectChange } from '@angular/material';
import { OrganisationType } from '../../shared/classes/enums';

@Component({
  selector: 'app-site-details',
  templateUrl: './site-details.component.html',
  styleUrls: ['./site-details.component.css']
})
export class SiteDetailsComponent extends CommonLoadingComponent {
  theForm: FormGroup;
  title = 'Site Details';
  allOrganisations: OrganisationMaster[];
  allCountries: Country[];
  countries: Country[];
  TimeZones: Country[];
  id = 0;
  showOrganisationSelection = true;
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
    this.getCountries();
    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      if (this.id > 0)
        this.title = 'Edit Site';
      else
        this.title = 'Create Site';
    });
  }

  onCancel() {
    this.router.navigate(['material-tracking', 'site-master']);
  }

  createForm() {
    this.theForm = this.fb.group({
      name: ['', Validators.required],
      description: '',
      country: this.showOrganisationSelection ? ['', Validators.required] : [''],
      timeZone: this.showOrganisationSelection ? ['', Validators.required] : [''],
      organisationID: this.showOrganisationSelection ? ['', Validators.required] : ['']
    });
  }

  toggle() {
    if (this.showOrganisationSelection) {
      this.theForm.controls['organisationID'].setValidators(Validators.required);
      this.theForm.controls['timeZone'].setValidators(Validators.required);
      this.theForm.controls['country'].setValidators(Validators.required);
    }
    else {
      this.theForm.controls['organisationID'].clearValidators();
      this.theForm.controls['timeZone'].clearValidators();
      this.theForm.controls['country'].clearValidators();
      this.TimeZones = null;
    }
    this.theForm.controls['organisationID'].setValue('');
    this.theForm.controls['country'].setValue('');
    this.theForm.controls['timeZone'].setValue('');
  }

  onCountryChanged(event: MatSelectChange) {
    if (event.value != undefined) {
      let allTimeZones = this.allCountries.filter(c => c.countryCode == event.value);
      this.TimeZones = Array<Country>();
      allTimeZones.forEach((item, index) => {
        if (!this.TimeZones.find(c => c.offset == item.offset)) {
          this.TimeZones.push(item);
        }
      });
      this.theForm.controls['timeZone'].setValue(this.TimeZones[0].offsetInMinutes);
  }
}

  onSubmit() {
    this.isLoading = true;
    if(!this.theForm.controls["timeZone"].value)
      this.theForm.controls["timeZone"].setValue(0);
    let site = this.theForm.value as SiteMaster;
    site.id = this.id;
    if (!this.showOrganisationSelection)
      site.organisationID = 0;
    if (this.id > 0) {
console.log(site);
      this.configurationService.updateSite(this.id, site).subscribe(data => {
        this.isLoading = false;
        if (!data["message"]) {
          this.router.navigate(['material-tracking', 'site-master']);
          this.uiUtilService.openSnackBar("Site updated Successfully", "OK");
        }
        else {
          this.uiUtilService.openSnackBar(data["message"], "OK");
        }

      }, error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      });
    }
    else {
      this.configurationService.createSite(site).subscribe(data => {
        this.isLoading = false;
        if (!data["message"]) {
          this.router.navigate(['material-tracking', 'site-master']);
          this.uiUtilService.openSnackBar("Site Created Successfully", "OK");
        }
        else {
          this.uiUtilService.openSnackBar(data["message"], "OK");
        }
      }, error => {
        this.isLoading = false;;
        this.uiUtilService.openSnackBar(error, "OK");
      });
    }
  }

  getCountries() {
    this.configurationService.getCountries().subscribe(
      data => {
        this.allCountries = data.sort(function (a, b) { return a.countryName.localeCompare(b.countryName) });
        this.countries = Array<Country>();
        this.allCountries.forEach((item, index) => {
          if (!this.countries.find(c => c.countryCode == item.countryCode)) {
            this.countries.push(item);
          }
        });
        this.getOrganisations();
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    )
  }
  getOrganisations() {
    this.materialTrackService.getListOrganisations().subscribe(
      data => {
        this.allOrganisations = data.filter(o => o.organisationType == OrganisationType.Vendor);
        if (this.id > 0) {
          this.getSiteDetails();
          if (this.accessRight < 2)
            this.title = 'Site Detail';
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

  getSiteDetails() {
    this.submit = "Update";
  
    this.configurationService.getSiteDetails(this.id).subscribe(
      data => {
      console.log(data);
     
        let allTimeZones = this.allCountries.filter(c => c.countryCode == data.country);
      this.TimeZones = Array<Country>();
    
      allTimeZones.forEach((item, index) => {
        if (!this.TimeZones.find(c => c.offset == item.offset)) {
          this.TimeZones.push(item);
        }
      });
        this.theForm.setValue({
          name: data.name,
          description: data.description,
          country: data.organisationID != 0 ? data.country : '',
          timeZone: data.organisationID != 0 ?  data['timeZoneOffset'] : 0,
          organisationID: data.organisationID == 0 ? data.organisationID.toString() : data.organisationID
        });
    
        this.showOrganisationSelection = data.organisationID == 0 ? false : true;
        if (!this.showOrganisationSelection) {
          this.theForm.controls['organisationID'].clearValidators();
          this.theForm.controls['timeZone'].clearValidators();
          this.theForm.controls['country'].clearValidators();
          this.TimeZones = null;
          this.theForm.controls['country'].setValue('');
          this.theForm.controls['country'].setValue('');
          this.theForm.controls['timeZone'].setValue('');
        }
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
