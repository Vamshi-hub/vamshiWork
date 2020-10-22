import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { FormGroup } from '@angular/forms';
import * as moment from 'moment';


import { MaterialTrackService } from '../material-track.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialDetail } from '../classes/material-detail';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { ProjectMaster } from '../classes/project-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { MaterialTrackHistory } from '../classes/material-track-history';

@Component({
  selector: 'app-material-details',
  templateUrl: './material-details.component.html',
  styleUrls: ['./material-details.component.css']
})
export class MaterialDetailsComponent extends CommonLoadingComponent {

  materialDetail: MaterialDetail;
  project: ProjectMaster;
  canEdit = false;
  editing = false;
  isShow=false;
  minExpectedDeliveryDate = moment();

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.route.paramMap.subscribe((params: ParamMap) => {
          const id = params.get('id');
          this.getMaterialDetail(+id);
        });
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No projects found', 'OK');
      }
    });
  }

  showCompare(id: number) {
    var index = this.materialDetail.trackingHistory.findIndex(m => m.id == id);
    let ids: number[] = new Array();
    ids.push(id);
    ids.push(this.materialDetail.trackingHistory[index + 1].id);

  }

  showCoursel(qcids: string) {
    this.router.navigate(['material-tracking', 'qc-defects', {qcids: qcids, title: this.materialDetail.block + "_" + this.materialDetail.level + "_" + this.materialDetail.zone}]);
  }

  onEditScheduleClicked() {
    this.editing = true;
  }

  onSaveScheduleClicked() {
    this.isLoading = true;
    this.materialTrackService.updateMaterialDetails(this.project.id, this.materialDetail).subscribe(
      response => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar("Updated successfully", "OK");
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      })
    this.editing = false;
  }

  onCancelClicked() {
    this.editing = false;
    this.isLoading = true;
    this.route.paramMap.subscribe((params: ParamMap) => {
      const id = params.get('id');
      this.getMaterialDetail(+id);
    });
  }

  getMaterialDetail(id: number) {
    this.materialTrackService.getMaterialDetail(this.project.id, id).subscribe(
      data => {
        this.materialDetail = data;
        if(this.materialDetail.length==null&&this.materialDetail.area==null&&this.materialDetail.dimensions==null){
          this.isShow=false;
        }
        else{
          this.isShow=true;
        }
        //this.materialDetail.trackingHistory.sort((a, b) => a.id < b.id ? -1 : a.id > b.id ? 1 : 0)
        // Check whether can change expected delivery date
        // Use hard coded status for now
        if (this.materialDetail.orderDate && this.materialDetail.trackingHistory.length > 0) {
          this.canEdit = true;
          this.minExpectedDeliveryDate = moment(this.materialDetail.orderDate).add(1, "days");
          this.materialDetail.trackingHistory.forEach((item, index) => {
            if (item.createdDate)
              this.canEdit = false;
          });
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );

  }

  QcPage(materialtracking: MaterialTrackHistory, qcType : number)
  {
if(qcType === 1)
{
  this.showCoursel(materialtracking.openQCCaseIds);
}
else if(qcType === 2)
{
  this.router.navigate(['../../material-qc', { module: this.materialDetail.block + "-"+this.materialDetail.level + "-" +this.materialDetail.zone + "-" + this.materialDetail.markingNo, stage: materialtracking.stageName }], { relativeTo: this.route })
}
else
{
  this.router.navigate(['/job-tracking', 'job-qc', { module: this.materialDetail.block + "-L"+this.materialDetail.level + "-" +this.materialDetail.zone + "-" + this.materialDetail.markingNo}], { relativeTo: this.route });
}
  }
}
