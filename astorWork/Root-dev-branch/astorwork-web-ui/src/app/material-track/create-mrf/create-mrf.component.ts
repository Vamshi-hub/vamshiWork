import { Component, OnInit } from '@angular/core';
import { MaterialTrackService } from '../material-track.service';
import { OrganisationMaster } from '../classes/organisation-master';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import * as moment from 'moment';

import { Mrf } from '../classes/mrf-master';
import { ProjectMaster } from '../classes/project-master';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ResultDlgComponent } from './result-dlg/result-dlg.component';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { ActivatedRoute, Router, NavigationStart } from '@angular/router';
import { MrfLocation } from '../classes/mrf-location';
import { MrfOrganisation } from '../classes/mrf-organisation';
import { OrganisationType } from '../../shared/classes/enums';

@Component({
  selector: 'app-create-mrf',
  templateUrl: './create-mrf.component.html',
  styleUrls: ['./create-mrf.component.css']
})
export class CreateMrfComponent implements OnInit {
  project: ProjectMaster
  theForm: FormGroup;
  submitted = false;
  blocks: string[];

  mrfLocations: MrfLocation[];
  mrfOrganisations: MrfOrganisation[];

  levels: Set<string>;
  zones: string[];
  /*
    mrf = {
      orderDate: moment(),
      block: '',
      level: '',
      zone: ''
    } as MrfMaster;
  */
  minOrderDate = moment();
  minExpectedDeliveryDate= moment().add(1,"days");

  organisations: OrganisationMaster[];
  allOrganisations: OrganisationMaster[];
  materialTypes: string[];

  showWho = false;
  showWhen = false;
  accessRight = 0;

  selectedBlocks: string[];

  constructor(private route: ActivatedRoute, private router: Router,
    private fb: FormBuilder, private materialTrackService: MaterialTrackService, private uiUtilService: UiUtilsService) { }

  ngOnInit() {
    this.route.data.subscribe(async (data: { accessRight: number }) => {
      this.accessRight = await (data.accessRight);
    });
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        //For fixing newley added blocks are not binded to blocks dropdown
        this.materialTrackService.getProjectInfo(this.project.id, "mrf").subscribe(
          data => {
            this.blocks=data.blocks;
          });
        this.getOrganisations();
      }
      else {
        this.uiUtilService.openSnackBar('No projects found', 'OK');
      }
    });

    this.createForm();
    this.router.events.subscribe((val) => {
      // If navigate away, close result dialog
      if (val instanceof NavigationStart) {
        this.uiUtilService.closeAllDialog();
      }
    });
  }

  createForm() {
    this.theForm = this.fb.group({
      orderDate: [moment(), Validators.required],
      plannedCastingDate: [''],
      expectedDeliveryDate: ['', Validators.required],
      block: ['', Validators.required],
      level: ['', Validators.required],
      zone: ['', Validators.required],
      materialTypes: ['', Validators.required],
      organisation: ['', Validators.required],
      officers: ['', Validators.required],
    });

    this.theForm.get('orderDate').valueChanges.subscribe(val => {
      if (val != null && this.theForm.controls.organisation.value != null)
        this.minExpectedDeliveryDate = moment(val).add(1, 'days');
    });

    this.theForm.get('plannedCastingDate').valueChanges.subscribe(val => {
      if (val != null && this.theForm.controls.organisation.value != null) {
        var newDate = moment(val).add(this.theForm.controls.organisation.value.cycleDays, 'days');
        if(newDate > moment(this.theForm.controls.orderDate.value))
          this.theForm.patchValue({
            expectedDeliveryDate: newDate
          });
      }
    });

    this.theForm.get('block').valueChanges.subscribe(val => {
      if (val != null)
        this.selectedBlocks = val;

      this.theForm.patchValue({
        level: null
      });
    });

    this.theForm.get('zone').valueChanges.subscribe(val => {
      if(this.zones != null && this.zones.length >=1 && val !== null && val !== "" && val.length>0)
        this.setDefaultVendorSelection();
    });

    this.theForm.get('level').valueChanges.subscribe(val => {
      if (this.mrfLocations && val != null) {
        const location = this.mrfLocations.filter((location) => val.indexOf(location.level) > -1);
        
        if (location.length > 0){
          let zonesSet = new Set(location[0].zones);
          
          location.forEach(item => {
            item.zones.forEach(item => {
              zonesSet.add(item)
            })
          }) 
          
          this.zones = Array.from(zonesSet);//location[0].zones;
          this.setDefaultSelection();

          if (this.zones != null && this.zones.length > 1)
            this.theForm.controls.zone.setValue(null);
        }
      }
    });

    this.theForm.get('organisation').valueChanges.subscribe(val => {
      if (val != null && this.mrfOrganisations.filter(mv => mv.organisationId == val.id).length > 0)
        this.materialTypes = this.mrfOrganisations.filter(mv => mv.organisationId == val.id)[0].materialTypes;
    });
  }

  setDefaultSelection(){
    //this.theForm.controls.zone.setValue(this.zones);

    if (this.zones != null && this.zones.length == 1){
      this.theForm.controls.zone.setValue(this.zones);
      this.setDefaultVendorSelection();
    }
  }


  setDefaultVendorSelection(){
    this.materialTrackService.getMaterialTypesForNewMRF(
      this.project.id,
      this.theForm.controls.block.value,
      this.theForm.controls.level.value,
      this.theForm.controls.zone.value).subscribe(
        data => {
          this.mrfOrganisations = data;
          let organisationIds = this.mrfOrganisations.map(mv => mv.organisationId);
          console.log("org ids: " + organisationIds);
          this.organisations = this.allOrganisations.filter(v => organisationIds.indexOf(v.id) >= 0 && v.organisationType == OrganisationType.Vendor);
          this.uiUtilService.closeAllDialog();

          if (this.organisations.length == 1){
            this.theForm.controls.organisation.setValue(this.organisations[0]);
            if (this.organisations[0].contactPeople.length == 1)
              this.theForm.controls.officers.setValue(this.organisations[0].contactPeople);
            if (this.mrfOrganisations[0].materialTypes.length == 1)
              this.theForm.controls.materialTypes.setValue(this.mrfOrganisations[0].materialTypes);
          }
        },
        error => {
          this.uiUtilService.closeAllDialog();
          this.uiUtilService.openSnackBar(error, 'OK');
        })
  }

  blockOpened(opened: boolean) {
    if (!opened && this.theForm.controls.block.value != null) {
      this.uiUtilService.openDialog(SpinnerDlgComponent, null, true);
      this.materialTrackService.getLocationForNewMRF(this.project.id, this.theForm.controls.block.value).subscribe(
        data => {
          this.mrfLocations = data;
          this.levels = new Set(this.mrfLocations.map((location) => location.level));
            if (this.levels.size == 1){
              let array = Array.from(this.levels);
              array.length = 1;
              this.theForm.controls.level.setValue(array);
          } 
          else{
               //this.zones = null;
             this.uiUtilService.closeAllDialog();
          }     
         
         
        },
        error => {
          this.uiUtilService.closeAllDialog();
          this.uiUtilService.openSnackBar(error, 'OK');
        });   
    }
  }

  zoneOpened(opened: boolean) {  
    var str=this.theForm.controls.zone.value;
    if (!opened && str!==null && str.length>0 && str!=='' && this.zones.length>1) {
      //this.uiUtilService.openDialog(SpinnerDlgComponent, null, true);
      this.setDefaultSelection();
    }

    this.theForm.patchValue({
      organisation: null,
      officers: null,
      materialTypes: null
    });
  }

  getOrganisations() {
    this.materialTrackService.getListOrganisations().subscribe(async data => {
      this.allOrganisations = await data;
    });
  }
  /*
    onSlabCastingDateChanged(event) {
      this.mrf.plannedCastingDate = moment(event.value);
      if (this.mrf.organisation != null) {
        this.mrf.expectedDeliveryDate = moment(event.value).add(this.mrf.organisation.cycleDays, 'days');
      }
    }
  
    onOrganisationSelectionChanged(event) {
      if (this.mrf.plannedCastingDate != null) {
        this.mrf.expectedDeliveryDate = moment(this.mrf.plannedCastingDate).add(event.value.cycleDays, 'days');
      }
    }
  */

  onSubmit() {
    this.submitted = true;
    let requestBody = this.theForm.value;
    requestBody['organisationId'] = requestBody.organisation.id;
    requestBody['officerUserIds'] = requestBody.officers === null ? [] : requestBody.officers.map((officer) => officer.id);
    requestBody['plannedCastingDate'] = requestBody.plannedCastingDate === null ? [] : requestBody.plannedCastingDate;
  
    //this.uiUtilService.openDialog(SpinnerDlgComponent, null, true);
    this.materialTrackService.createMRF(this.project.id, requestBody).subscribe(
      response => {
        let data = {};
        
        if (response['mrfNo']) {
          data['success'] = true;
          data['blk'] = requestBody['block'];
          data['mrfNo'] = response['mrfNo'];
          data['materialCount'] = response['materialCount'];
        }
        else {
          data['success'] = false;
          data['message'] = response['message'];
        }
        //this.uiUtilService.closeAllDialog();
        this.uiUtilService.openDialog(ResultDlgComponent, data, true);
        this.submitted = false;
      },
      error => {
        //this.uiUtilService.closeAllDialog();
        this.uiUtilService.openSnackBar(error, 'OK');
        this.submitted=false;
      });
  }

  OnCancel() {
    this.router.navigateByUrl('/material-tracking/mrfs');
  }
}
