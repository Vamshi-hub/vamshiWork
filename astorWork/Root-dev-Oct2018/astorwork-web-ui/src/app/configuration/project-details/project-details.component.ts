import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, FormControl } from '@angular/forms';
import { Users } from '../../material-track/classes/users';
import { ProjectMaster } from '../../material-track/classes/project-master';
import { ActivatedRoute, Router, ParamMap } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import * as moment from 'moment';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { DailyStatus } from '../../material-track/classes/dailyStatus';
import { ConfigurationService } from '../configuration.service';
import { Country } from '../classes/country';
import { MatSelectChange } from '@angular/material';

@Component({
  selector: 'app-project-details',
  templateUrl: './project-details.component.html',
  styleUrls: ['./project-details.component.css']
})
export class ProjectDetailsComponent extends CommonLoadingComponent {
  theForm: FormGroup;
  submit = "Save";
  projectDetails: ProjectMaster;
  id: number;
  errorMsg = "";
  title = "Project Details"
  minStartDate = moment();
  minEndDate = moment().add(1, "days");

  allCountries: Country[];
  countries: Country[];
  TimeZones: Country[];
  editing = true;
  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder,
     private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService,
     private configurationService: ConfigurationService) {
    super(route, router);
   }

  ngOnInit() {
    super.ngOnInit();

    this.route.paramMap.subscribe((params: ParamMap) => {
      this.id = parseInt(params.get('id'));
      this.getCountries();
      if (this.id > 0)
      {
         this.title = 'Edit Project';
      }
      else
      {
        this.title = 'Create Project';
        //this.isLoading = false;
      }
    });
    this.createForm();
  }

  createForm() {
    this.theForm = this.fb.group({
      projectName: ['', Validators.required],
      description: [''],
      country: ['',Validators.required],
      timeZone: ['',Validators.required],
      startDate: ['', Validators.required],
      endDate: ['']
    });
    this.theForm.get('startDate').valueChanges.subscribe(val => {
      if (val != null) {
        if(this.minEndDate < moment(val).add(1,'days'))
        this.minEndDate = moment(val).add(1, 'days');
      }
    });
  }

  OnCancel() {
    this.router.navigateByUrl('/configuration/project-master');
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
        if(this.id > 0)
        this.getProjectDetailsById(this.id)
        else
        {
          this.isLoading = false;
        }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    )
  }

  onSubmit() {
    let requestBody = this.theForm.value;
    let project: ProjectMaster;
    project = new ProjectMaster();
    project.id = this.id;
    project.name = requestBody['projectName'];
    project.description = requestBody['description'];
    project.country = requestBody['country'];
    project.timeZoneOffset = requestBody['timeZone'] != null ? requestBody['timeZone'] : 0;
    project.startDate = requestBody['startDate'];
    project.endDate = requestBody['endDate'];
    this.isLoading = true;
    (this.id == 0 ? this.materialTrackService.createProject(project) : this.materialTrackService.updateProject(this.id, project)).subscribe(
      response => {
        let data = {};
        if (response == null) {
          data['success'] = true;
          data['message'] = " ";
          if (this.id == 0)
            data['message'] = "Project Created Successfully.";
          else
            data['message'] = "project Updated Successfully. ";

          this.uiUtilService.openSnackBar(data['message'], "OK");
          this.router.navigateByUrl('/configuration/project-master');
        }
        else {
          data['success'] = false;
          data['message'] = response['message'];
          this.errorMsg = data["message"];
          this.uiUtilService.openSnackBar(this.errorMsg, "OK");
          this.theForm.controls['projectName'].setValue('');
        }
        this.isLoading = false;
      },
      error => {
        this.errorMsg = error["message"];
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });


  }

  getProjectDetailsById(projectId: number) {
    this.submit = "Update";
    this.editing=false;
    this.materialTrackService.getProjectInfo(projectId).subscribe(
      data => {
        this.projectDetails = data;
        let allTimeZones = this.allCountries.filter(c => c.countryCode == data.country);
      this.TimeZones = Array<Country>();
      allTimeZones.forEach((item, index) => {
        if (!this.TimeZones.find(c => c.offset == item.offset)) {
          this.TimeZones.push(item);
        }
      });
      console.log(this.projectDetails);
        this.theForm.controls['projectName'].setValue(this.projectDetails.name);
        this.theForm.controls['description'].setValue(this.projectDetails.description);
        this.theForm.controls['startDate'].clearValidators();
        this.theForm.controls['startDate'].setValue(this.projectDetails.startDate);
        this.theForm.controls['endDate'].setValue(this.projectDetails.endDate);
        this.theForm.controls['country'].setValue(this.projectDetails.country);
        this.theForm.controls['timeZone'].setValue(this.projectDetails.timeZoneOffset);
        //this.theForm.controls['']
        if(this.accessRight < 2 )
        this.title = 'Project Details';
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }
}
