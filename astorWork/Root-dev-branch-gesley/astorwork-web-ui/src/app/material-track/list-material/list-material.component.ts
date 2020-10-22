import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { Observable } from 'rxjs';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSort, MatPaginator, MatTableDataSource, MatSelectChange, MAT_EXPANSION_PANEL_DEFAULT_OPTIONS } from '@angular/material';

import { MaterialTrackService } from '../material-track.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialMaster } from '../classes/material-master';
import { ProjectMaster } from '../classes/project-master';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import * as moment from 'moment';
import { equal } from 'assert';
import { ShowQrcodeComponent } from './show-qrcode/show-qrcode.component';
@Component({
  selector: 'app-list-material',
  templateUrl: './list-material.component.html',
  styleUrls: ['./list-material.component.css']
})

export class ListMaterialComponent extends CommonLoadingComponent {
  displayedColumns = ['markingNo', 'block', 'level', 'zone', 'materialType', 'mrfNo', 'expectedDeliveryDate', 'qcStatus', 'stageName', 'actions'];
  dataSource: MatTableDataSource<MaterialMaster> = new MatTableDataSource();
  lstMaterials: MaterialMaster[];
  project: ProjectMaster;
  blocks = [];
  selectedBlock = '';
  filterMRF = '';
  selectedDeliveryFilter = "0";
  filterStage = '';
  filterDelivery = '';
  stageorder = 0;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.dataSource = new MatTableDataSource();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.materialTrackService.getProjectInfo(this.project.id, "").subscribe(
          data => {
            this.blocks = data.blocks;
          });
        this.route.paramMap.subscribe((params: Params) => {
          this.selectedBlock = params.get('blk');       
            this.filterMRF = params.get('mrfNo') == null ? '' : params.get('mrfNo');       
            this.stageorder = params.get('stagenum') == 0 ? 0 : params.get('stagenum');

          this.selectedDeliveryFilter = params.get('selectedDeliveryFilter') == null ? '0' : params.get('selectedDeliveryFilter')

          if (this.filterMRF.length > 0 || this.selectedDeliveryFilter.length > 0)
            this.selectedBlock = "All";
          else
            this.selectedBlock = this.blocks[0];

          if (this.selectedBlock)
            this.getMaterialsByBlk(this.selectedBlock);
          else if (this.selectedBlock = "All")
            this.getMaterialsByBlk(this.selectedBlock);
          else {
            this.selectedBlock = window.localStorage.getItem('blk');
            if (this.selectedBlock)
              this.getMaterialsByBlk(this.selectedBlock);
            else {
              this.selectedBlock = this.blocks[0];
              this.getMaterialsByBlk(this.selectedBlock);
            }
          }
        });
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No project selected', 'OK');
      }
    });
  }

  onFilterKeyIn(event: KeyboardEvent) {
    var filterValue = (<HTMLInputElement>event.target).value;
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/material-tracking/materials/' + row['id']);
  }

  onBlockChanged(event: MatSelectChange) {
    let message = '';
    let deliveryStageOrder = this.lstMaterials[0].deliveryStageOrder;
    if (event.value != undefined) {
      this.dataSource = null;
      this.isLoading = true;
      this.getMaterialsByBlk(event.value);
    }
    else {
      this.uiUtilService.openSnackBar('Block not selected!', 'OK');
    }
  }

  onViewBIMClicked(material: MaterialMaster) {
    sessionStorage.setItem('model_urn', material.forgeModelURN);
    sessionStorage.setItem('element_id', String(material.forgeElementID));
    this.router.navigate(['forge-viewer']);
  }

  onDeliveryStatusChanged(event: MatSelectChange) {
    this.selectedDeliveryFilter = event.value;
    this.applyFilteToDataSource();
  }

  getMaterialsByBlk(blk: string) {
    this.materialTrackService.getMaterials(this.project.id, blk).subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        this.lstMaterials = data;
        console.log(this.lstMaterials);
        if (this.stageorder != 0)
          this.lstMaterials = this.lstMaterials.filter(s => s.stageOrder >= this.stageorder)
        this.applyFilteToDataSource();
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  applyFilteToDataSource() {
    let message = '';
    if (this.selectedDeliveryFilter == "0")
      this.dataSource = new MatTableDataSource(this.lstMaterials);
    else if (this.selectedDeliveryFilter == "1") {
      this.dataSource = new MatTableDataSource(this.lstMaterials.filter(m => m.expectedDeliveryDate != null && m.stageOrder < m.deliveryStageOrder && moment(m.expectedDeliveryDate).startOf('day') >= moment().startOf('day') && moment(m.expectedDeliveryDate).startOf('day') <= moment().endOf('day')));
      message = "Today there is no delivery.";
    }
    else if (this.selectedDeliveryFilter == "2") {
      this.dataSource = new MatTableDataSource(this.lstMaterials.filter(m => m.expectedDeliveryDate != null && m.stageOrder < m.deliveryStageOrder && moment(m.expectedDeliveryDate) < moment().startOf('day')));
      message = "No delayed delivery material.";
    }

    if (this.dataSource.data.length > 0) {
      this.dataSource.paginator = this.paginator;
      this.dataSource.sort = this.sort;
      this.dataSource.filterPredicate = (data: MaterialMaster, filter: string) => {
        const filterArray = filter.split(' ');
        let countMatch = 0;
        for (let entry of filterArray) {
          if (data.level.toLowerCase().indexOf(entry) >= 0 ||
            data.markingNo.toLowerCase().indexOf(entry) >= 0 ||
            data.zone.toLowerCase().indexOf(entry) >= 0 ||
            data.materialType.toLowerCase().indexOf(entry) >= 0 ||
            (data.stageName != null && data.stageName.toLowerCase().indexOf(entry) >= 0) ||
            (data.qcStatus.toLowerCase().indexOf(entry) >= 0) ||
            (data.mrfNo != null && data.mrfNo.toLowerCase().indexOf(entry) >= 0))
            countMatch++;
        }
        if (countMatch == filterArray.length)
          return true;
        else
          return false;
      };
      if (this.filterMRF != '') {
        this.filterMRF = this.filterMRF.toLowerCase();
        this.dataSource.filter = this.filterMRF;
      }

      window.localStorage.setItem('blk', this.selectedBlock);
    }
    else {
      if (this.selectedDeliveryFilter)
        this.uiUtilService.openSnackBar(message, 'OK');
      else
        this.uiUtilService.openSnackBar("This block doesn't have any materials", 'OK');
    }
  }

  ShowQcCase(material: MaterialMaster) {
    this.router.navigate(['material-tracking', 'qc-defects', { qcids: material.openQCCaseID.toString(), title: material.block + "_" + material.level + "_" + material.zone }]);
  }

  onViewQRClicked(material: MaterialMaster) {
    let data = {};
    this.uiUtilService.openDialog(ShowQrcodeComponent, data, true);
    data['project'] = this.project;
    this.materialTrackService.getMaterialDetail(this.project.id, material.id).subscribe(
      response => {
        data['material'] = response;
        this.uiUtilService.openDialog(ShowQrcodeComponent, data, true);
      });
  }
  showAllMaterials() {
    this.stageorder = 0;
    this.filterMRF='';
    this.getMaterialsByBlk(this.selectedBlock);
  }
}