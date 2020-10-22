import { Component, OnInit, ViewChild, Input } from '@angular/core';

import { animate, state, style, transition, trigger } from '@angular/animations';

import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSort, MatPaginator, MatTableDataSource, MatSelectChange, MatTable } from '@angular/material';

import { MaterialTrackService } from '../material-track.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { QCDefect } from '../classes/qc-defect';
import { ProjectMaster } from '../classes/project-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { SlideShowComponent } from './slide-show/slide-show.component';
import { QCPhoto } from '../classes/qc-photo';
import { QCCase } from '../classes/qc-case';

@Component({
  selector: 'app-list-defect',
  templateUrl: './list-defect.component.html',
  styleUrls: ['./list-defect.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})

export class ListDefectComponent extends CommonLoadingComponent {
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  columnsToDisplay = ['isOpen', 'caseName', "markingNo", "stageName", 'createdDate', 'createdBy', 'updatedDate', 'updatedBy', 'duration', 'progress'];
  caseDataSource: MatTableDataSource<QCCase> = new MatTableDataSource();
  listCases: QCCase[];
  expandedElement: QCDefect[];

  displayedColumns = ['isOpen', 'remarks', 'Photo', 'createdDate', 'createdBy', 'updatedDate', 'updatedBy'];
  dataSource: MatTableDataSource<QCDefect> = new MatTableDataSource();
  lstqcDefect: QCDefect[];

  project: ProjectMaster;
  qcPhoto: QCPhoto[]
  caseId = 0;
  filterCase = '';
  Math: any;
  slideShowData: SlideShowData;
  isDefectLoading = false;
  title = "QC Cases";
  displayShowAll = false;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.Math = Math;
    this.dataSource = new MatTableDataSource();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;

        await this.route.paramMap.subscribe((params: Params) => {
          this.caseId = params.get('id');
          this.filterCase = params.get('caseName');

          if (this.caseId)
            this.title = params.get('title') == null ? 'QC Cases' : ' QC Cases (' + params.get('title') + ')';

          this.getCases(project.id);
          if (this.caseId > 0 || this.filterCase)
            this.displayShowAll = true;
        });
        //await this.uiUtilService.openSnackBar(this.caseDataSource.filteredData.length.toString(), 'OK');
      }
    });
  }

  onFilterKeyIn(event: KeyboardEvent) {
    var filterValue = (<HTMLInputElement>event.target).value;
    if (filterValue == "open")
      filterValue = "true";
    if (filterValue == "closed")
      filterValue = "false";
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.caseDataSource.filter = filterValue;

    if (this.caseDataSource.filteredData.length < this.caseDataSource.data.length || this.title.length > "QC Cases".length)
      this.displayShowAll = true;
    else
      this.displayShowAll = false;
  }

  selectRow(row: any) {
    if (row['countPhotos'] > 0)
      this.getPhoto(row['id'], row['remarks']);

  }

  selectCaseRow(row: any) {
    this.dataSource = null;
    if (row == this.expandedElement) {
      this.expandedElement = Array<QCDefect>();
      return false;
    }
    else {
      this.isDefectLoading = true;
      this.getDefectsByCase(row['id']);
      return row;
    }
  }

  allCases() {

    this.caseDataSource = new MatTableDataSource(this.listCases);
    this.filterCase = '';
    this.caseDataSource.paginator = this.paginator;
    this.caseDataSource.sort = this.sort;
    this.title = "QC Cases";
    this.displayShowAll = false;
  }

  getPhoto(defectID: number, remarks: string) {
    this.isLoading = true;
    this.materialTrackService.getQCPhotos(defectID).subscribe(data => {
      this.qcPhoto = data;

      if (this.qcPhoto != null) {

        this.slideShowData = new SlideShowData();
        this.slideShowData.qcOpenPhotos = this.qcPhoto.filter(p => p.isOpen);
        this.slideShowData.defectID = defectID;
        this.slideShowData.remarks = remarks;
        this.slideShowData.qcClosePhotos = this.qcPhoto.filter(p => p.isOpen == false);
        this.isLoading = false;
        this.uiUtilService.openDialog(SlideShowComponent, this.slideShowData, true);
      }
    })
  }

  getDefectsByCase(case_id: number) {
    this.materialTrackService.getQCDefects(case_id).subscribe(
      data => {
        this.lstqcDefect = data;
        this.dataSource = new MatTableDataSource(data);
        this.isDefectLoading = false;
        this.isLoading = false;
      },
      error => {
        this.isDefectLoading = false;
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  getCases(projectID: number) {
    this.materialTrackService.getCases(projectID).subscribe(
      data => {
        this.listCases = data;

        if (this.caseId != null && this.caseId > 0) {
          this.caseDataSource = new MatTableDataSource(data.filter(d => d.id == this.caseId));
        }
        else {
          this.caseDataSource = new MatTableDataSource(data.sort().reverse());
        }
        this.caseDataSource.paginator = this.paginator;
        this.caseDataSource.sort = this.sort;

        //this.filterCase = this.filterCase.toLowerCase();

        this.caseDataSource.filter = this.filterCase;
        if (this.filterCase == "open")
          this.caseDataSource.filter = "true";
        if (this.filterCase == "closed")
          this.caseDataSource.filter = "false";
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }
}
export class SlideShowData {
  defectID: number;
  remarks: string;
  qcOpenPhotos: QCPhoto[];
  qcClosePhotos: QCPhoto[];
}
