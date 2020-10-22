import { Component, OnInit } from '@angular/core';
import { MaterialTrackService } from '../material-track.service';
import { VendorMaster } from '../classes/vendor-master';
import { FormBuilder, FormGroup, FormControl, Validators } from '@angular/forms';
import * as moment from 'moment';

import { MrfMaster } from '../classes/mrf-master';
import { ProjectMaster } from '../classes/project-master';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ResultDlgComponent } from './result-dlg/result-dlg.component';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { ActivatedRoute, Router, NavigationStart } from '@angular/router';
import { MrfLocation } from '../classes/mrf-location';
import { MrfVendor } from '../classes/mrf-vendor';

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
  mrfVendors: MrfVendor[];

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

  vendors: VendorMaster[];
  allVendors: VendorMaster[];
  materialTypes: string[];

  showWho = false;
  showWhen = false;
  accessRight = 0;
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
        this.blocks = this.project.blocks;
        this.getVendors();
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
      plannedCastingDate: ['', Validators.required],
      expectedDeliveryDate: ['', Validators.required],
      block: ['', Validators.required],
      level: ['', Validators.required],
      zone: ['', Validators.required],
      materialTypes: ['', Validators.required],
      vendor: ['', Validators.required],
      officers: ['', Validators.required],
    });

    this.theForm.get('orderDate').valueChanges.subscribe(val => {
      if (val != null && this.theForm.controls.vendor.value != null) {
        this.minExpectedDeliveryDate = moment(val).add(1, 'days');
      }
    });

    this.theForm.get('plannedCastingDate').valueChanges.subscribe(val => {
      if (val != null && this.theForm.controls.vendor.value != null) {
        var newDate = moment(val).add(this.theForm.controls.vendor.value.cycleDays, 'days');
        if(newDate>moment(this.theForm.controls.orderDate.value))
        {
        this.theForm.patchValue({
          expectedDeliveryDate: newDate
        });
      }
      }
    });

    this.theForm.get('block').valueChanges.subscribe(val => {
      if (val != null) {
        this.uiUtilService.openDialog(SpinnerDlgComponent, null, true);
        this.materialTrackService.getLocationForNewMRF(this.project.id, val).subscribe(
          data => {
            this.mrfLocations = data;
            this.levels = new Set(this.mrfLocations.map((location) => location.level));
            this.zones = null;
            this.uiUtilService.closeAllDialog();
          },
          error => {
            this.uiUtilService.closeAllDialog();
            this.uiUtilService.openSnackBar(error, 'OK');

          });
      }

      this.theForm.patchValue({
        level: null
      });
    });

    this.theForm.get('level').valueChanges.subscribe(val => {
      if (this.mrfLocations) {
        const location = this.mrfLocations.filter((location) => location.level == val);
        if (location.length > 0)
          this.zones = location[0].zones;
      }
      this.theForm.patchValue({
        zone: null
      });
    });

    this.theForm.get('zone').valueChanges.subscribe(val => {
      if (val != null) {
        this.uiUtilService.openDialog(SpinnerDlgComponent, null, true);
        this.materialTrackService.getMaterialTypesForNewMRF(
          this.project.id,
          this.theForm.controls.block.value,
          this.theForm.controls.level.value,
          this.theForm.controls.zone.value).subscribe(
            data => {
              this.mrfVendors = data;
              this.vendors = this.allVendors.filter(v => this.mrfVendors.map(mv => mv.vendorID).indexOf(v.id) >= 0);
              this.uiUtilService.closeAllDialog();

            },
            error => {
              this.uiUtilService.closeAllDialog();
              this.uiUtilService.openSnackBar(error, 'OK');
            })
      }

      this.theForm.patchValue({
        vendor: null,
        officers: null,
        materialTypes: null
      });
    });

    this.theForm.get('vendor').valueChanges.subscribe(val => {
      if (val != null) {
        this.materialTypes = this.mrfVendors.filter(mv => mv.vendorID == val.id)[0].materialTypes;
        if (this.theForm.controls.plannedCastingDate.value != null) {
          var newDate = moment(this.theForm.controls.plannedCastingDate.value).add(val.cycleDays, 'days');
          this.theForm.patchValue({
            expectedDeliveryDate: newDate
          });
        }
      }
    });
  }

  getVendors() {
    this.materialTrackService.getListVendors().subscribe(async data => {
      this.allVendors = await data;
    });
  }
  /*
    onSlabCastingDateChanged(event) {
      this.mrf.plannedCastingDate = moment(event.value);
      if (this.mrf.vendor != null) {
        this.mrf.expectedDeliveryDate = moment(event.value).add(this.mrf.vendor.cycleDays, 'days');
      }
    }
  
    onVendorSelectionChanged(event) {
      if (this.mrf.plannedCastingDate != null) {
        this.mrf.expectedDeliveryDate = moment(this.mrf.plannedCastingDate).add(event.value.cycleDays, 'days');
      }
    }
  */
  onSubmit() {
    this.submitted = true;
    let requestBody = this.theForm.value;
    requestBody['vendorId'] = requestBody.vendor.id;
    requestBody['officerUserIds'] = requestBody.officers === null ? [] : requestBody.officers.map((officer) => officer.id);

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
      });
  }

  OnCancel() {
    this.router.navigateByUrl('/material-tracking/mrfs');
  }
}
