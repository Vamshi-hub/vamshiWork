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
import { QCPhoto } from '../../job-track/classes/qc-photo';
import { QCCase } from '../classes/qc-case';
import * as moment from 'moment';
import { SelectionChange } from '@angular/cdk/collections';
//import moment = require('moment');
//import { start } from 'repl';

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
  minDate = new Date();
  columnsToDisplay = ['isOpen','caseName', "markingNo", 'createdDate', 'createdBy', 'updatedDate', 'updatedBy', 'duration', 'progress'];
  caseDataSource: MatTableDataSource<QCCase> = new MatTableDataSource();
  listCases: QCCase[];
  expandedElement: QCDefect[];

  displayedColumns = ['statusCode', 'remarks', 'Photo', 'createdDate', 'createdBy','updatedDate', 'updatedBy'];
  dataSource: MatTableDataSource<QCDefect> = new MatTableDataSource();
  lstqcDefect: QCDefect[];

  project: ProjectMaster;
  qcPhoto: QCPhoto[]
  caseIds = '';
  filterValue = '';
  Math: any;
  slideShowData: SlideShowData;
  isDefectLoading = false;
  title = "QC Cases";
  displayShowAll = false;
  startDate: moment.Moment;
  endDate: moment.Moment;

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
          this.caseIds = params.get('qcids');
          this.filterValue = params.get('filter');

          if (this.caseIds)
            this.title = params.get('title') == null ? 'QC Cases' : ' QC Cases (' + params.get('title') + ')';

          if ((this.caseIds != null && this.caseIds != '') || this.filterValue)
            this.displayShowAll = true;

          this.getCases(project.id);
        });
        //await this.uiUtilService.openSnackBar(this.caseDataSource.filteredData.length.toString(), 'OK');
      }
    });

    this.filterQCCases();
  }

  onFilterKeyIn(event: KeyboardEvent) {
    this.filterValue = (<HTMLInputElement>event.target).value;
    this.caseDataSource.filterPredicate = (data: QCCase, filter: string) => {
      const filterArray = filter.split(' ');
      let countMatch = 0;
      for (let entry of filterArray) {
        entry = entry.toLowerCase();
        if (data.caseName.toLowerCase().indexOf(entry) >= 0 ||
            data.markingNo.toLowerCase().indexOf(entry) >= 0 ||
            data.createdBy.toLowerCase().indexOf(entry) >= 0 ||
            //data.updatedBy.toLowerCase().indexOf(entry) >= 0 ||
            data.duration.toLowerCase().indexOf(entry) >= 0 ||
            data.progress > parseInt(entry))
          countMatch++;
      }
      if (countMatch >= filterArray.length)
        return true;
      else
        return false;
    };
    this.filterQCCases();
  }

  onStartDateChanged(event: MatSelectChange) {
    this.startDate = event.value;
    this.filterQCCases();
  }

  onEndDateChanged(event: MatSelectChange) {
    this.endDate = event.value;
    this.filterQCCases();
  }

  selectRow(row: any) {
    if (row['countPhotos'] > 0)
      this.getPhoto(row['id'], row['remarks'],row['createdBy']);
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

  showAllCases() {
    this.caseDataSource = new MatTableDataSource(this.listCases);
    this.filterValue = '';
    this.caseDataSource.paginator = this.paginator;
    this.caseDataSource.sort = this.sort;
    this.title = "QC Cases";
    this.displayShowAll = false;
    this.startDate = null;
    this.endDate = null;
  }

  getPhoto(defectID: number, remarks: string,createdBy:string) {
    this.isLoading = true;
    this.materialTrackService.getQCPhotos(defectID).subscribe(data => {
      this.qcPhoto = data;
      if (this.qcPhoto != null) {
        this.slideShowData = new SlideShowData();
        this.slideShowData.qcOpenPhotos = this.qcPhoto.filter(p => p.isOpen);
        this.slideShowData.defectID = defectID;
        this.slideShowData.remarks = remarks;
        this.slideShowData.createdBy=createdBy;
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
        if (this.caseIds != null && this.caseIds != '') {
          var arr = this.caseIds.split(',');
          this.caseDataSource = new MatTableDataSource(data.filter(d => arr.includes(d.id.toString())));
        }
        else {
          this.caseDataSource = new MatTableDataSource(data.sort().reverse());
        }

        this.filterByCaseName();
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  filterQCCases() {
    if (this.startDate && this.endDate)
      this.filterByDate();

    if (this.filterValue)
      this.filterByFilter(this.filterValue);
    this.caseDataSource.filter = this.filterValue;
    this.caseDataSource.paginator = this.paginator;
    this.caseDataSource.sort = this.sort;
  }

  filterByCaseName() {
    let filter = null;
    if (this.filterValue == "open")
      filter = "true";
    if (this.filterValue == "closed")
      filter = "false";

    if (filter) {
      this.caseDataSource.filter = filter;
      this.displayShowAll = true;
    }

    this.caseDataSource.paginator = this.paginator;
    this.caseDataSource.sort = this.sort;
  }

  filterByFilter(filterValue: string) {
    if (filterValue == "open")
      filterValue = "true";
    if (filterValue == "closed")
      filterValue = "false";
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.caseDataSource.filter = filterValue;

    if (this.caseIds || this.filterValue)
      this.displayShowAll = true;

    if (this.caseDataSource.filteredData.length < this.caseDataSource.data.length || this.title.length > "QC Cases".length)
      this.displayShowAll = true;
  }

  filterByDate() {
    console.log(this.listCases);
    this.caseDataSource = new MatTableDataSource(
      this.listCases.filter(qcCase =>
        moment(qcCase.createdDate).isBetween(this.startDate, this.endDate, 'day', '[]')
      )
    );
    this.displayShowAll = true;
  }
}

export class SlideShowData {
  defectID: number;
  createdBy:string;
  remarks: string; keys
  qcOpenPhotos: QCPhoto[];
  qcClosePhotos: QCPhoto[];
 
}

