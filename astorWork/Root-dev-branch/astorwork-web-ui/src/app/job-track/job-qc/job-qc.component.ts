import { Component, OnInit, ViewChild, Input } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSort, MatPaginator, MatTableDataSource, MatSelectChange, MatTable } from '@angular/material';
import { Jobstarted } from '../classes/jobstarted';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ProjectMaster } from '../classes/project-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { JobSlideShowComponent } from './slide-show/slide-show.component';
import { QCPhoto } from '../classes/qc-photo';
import * as moment from 'moment';
import { JobTrackService } from '../job-track.service';
import { filter } from 'rxjs/operators';


@Component({
  selector: 'app-job-qc',
  templateUrl: './job-qc.component.html',
  styleUrls: ['./job-qc.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})


export class JobQCComponent extends CommonLoadingComponent {
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  minDate = new Date();
  columnsToDisplay = ['status', 'tradeName', "markingNo", 'plannedStartDate', 'plannedEndDate', 'qcStartDate', 'qcEndDate'];
  caseDataSource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  listCases: Jobstarted[];
  finallist: Jobstarted[];
  expandedElement: Jobstarted[];

  displayedColumns = ['checklistStatus', 'countPhotos', 'checkListName', 'qcFailedBy'];
  dataSource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  lstqcDefect: Jobstarted[];
  project: ProjectMaster;
  qcPhoto: QCPhoto[];
  caseIds = '';
  jobid: number;
  id = '';
  filterValue = '';
  Math: any;
  slideShowData: SlideShowData;
  isDefectLoading = false;
  title = "QC Cases";
  displayShowAll = false;
  startDate: moment.Moment;
  endDate: moment.Moment;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private jobtrackservice: JobTrackService) {
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
          this.jobid = params.get('qcids');
          this.filterValue = params.get('filter');
          if ((this.jobid != null) || this.filterValue)
            this.displayShowAll = true;
          this.getarchiqcjobschecklists(project.id);
        });
      }
    });


    this.filterQCCases();
  }

  onFilterKeyIn(event: KeyboardEvent) {
    this.filterValue = (<HTMLInputElement>event.target).value;
    this.caseDataSource.filterPredicate = (data: Jobstarted, filter: string) => {
      const filterArray = filter.split(' ');
      let countMatch = 0;
      for (let entry of filterArray) {
        entry = entry.toLowerCase();
        if (data.tradeName.toLowerCase().indexOf(entry) >= 0 ||
          data.status.toLowerCase().indexOf(entry) >= 0 ||
          data.markingNo.toLowerCase().indexOf(entry) >= 0)
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
      this.getPhoto(row['checklistID'], this.project.id, row['id'], 0);


  }

  selectJobRow(row: any) {
    this.dataSource = null;
    if (row == this.expandedElement) {
      this.expandedElement = Array<Jobstarted>();
      return false;
    }
    else {
      this.isDefectLoading = true;
      this.getchecklistByJob(row['id'], this.project.id);
      return row;
    }
  }

  showAllCases() {
    this.jobtrackservice.getarchiqcjobschecklists(this.project.id).subscribe(
      data => {
        this.listCases = new Array();
        this.lstqcDefect = data;
        var results :Jobstarted[];
        results=data;
        for(var i=0;i<=data.length-1;i++) {

          if (this.listCases.length == 0 || this.listCases.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType).length === 0) {

            var ss = data.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType)
            var aa: Jobstarted=new Jobstarted();
            aa.tradeName = data[i].tradeName;

            aa.markingNo = data[i].markingNo;
            aa.status = data[i].status.replace(/_/g, ' ');
            aa.id = data[i].id;
            aa.qcStartDate = ss[0].qcStartDate;
            aa.qcEndDate = ss[ss.length - 1].qcEndDate;
            aa.plannedStartDate = data[i].plannedStartDate;
            aa.plannedEndDate = data[i].plannedEndDate;
            aa.block = data[i].block;
            aa.level = data[i].level;
            aa.materialType = data[i].materialType;
            this.listCases.push(aa);

          }
          }
        this.caseDataSource = new MatTableDataSource(this.listCases);

        this.caseDataSource.paginator = this.paginator;
        this.caseDataSource.sort = this.sort;
        this.displayShowAll = false;
        this.filterValue='';
        this.startDate = null;
        this.endDate = null;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      });
  }

  getPhoto(checklistID: number, projectID: number, jobscheduleID: number, MaterialStageAuditID: number) {
    this.isLoading = true;
    this.jobtrackservice.getQCPhotos(projectID, checklistID, jobscheduleID, MaterialStageAuditID).subscribe(data => {
      this.qcPhoto = data;
      if (this.qcPhoto != null) {
        this.slideShowData = new SlideShowData();
        this.slideShowData.qcOpenPhotos = this.qcPhoto;
        this.isLoading = false;
        this.uiUtilService.openDialog(JobSlideShowComponent, this.slideShowData, true);
      }
    })
  }


  getchecklistByJob(case_id: number, projectId: number) {
    this.dataSource = new MatTableDataSource(this.lstqcDefect.filter(d => d.id == case_id));
    if (this.dataSource.data.length > 0) {
      this.dataSource.data.forEach(item => {
        item.checklistStatus = item.checklistStatus.replace(/_/g, ' ');
      })
    }
    this.isDefectLoading = false;
    this.isLoading = false;
  }


  getarchiqcjobschecklists(projectID: number) {
    this.jobtrackservice.getarchiqcjobschecklists(projectID).subscribe(
      data => {
        this.listCases = new Array();
        this.lstqcDefect = data;
        var results :Jobstarted[];
        results=data;
        for(var i=0;i<=data.length-1;i++) {

          if (this.listCases.length == 0 || this.listCases.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType).length === 0) {

            var ss = data.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType)
            var aa: Jobstarted=new Jobstarted();
            aa.tradeName = data[i].tradeName;
            aa.markingNo = data[i].markingNo;
            aa.status = data[i].status.replace(/_/g, ' ');
            aa.id = data[i].id;
            aa.qcStartDate = ss[0].qcStartDate;
            aa.qcEndDate = ss[ss.length - 1].qcEndDate;
            aa.plannedStartDate = data[i].plannedStartDate;
            aa.plannedEndDate = data[i].plannedEndDate;
            aa.block = data[i].block;
            aa.level = data[i].level;
            aa.materialType = data[i].materialType;
            this.listCases.push(aa);
            
          }
          }
          if(this.filterValue){
            this.caseDataSource = new MatTableDataSource(this.listCases.filter(s=>s.status==this.filterValue));
          }
          else if(this.jobid != null)
          {
           this.listCases=this.listCases.filter(s=>s.id==this.jobid)
           this.caseDataSource = new MatTableDataSource(this.listCases);
           this.displayShowAll=true;
          }
          else{
            this.caseDataSource = new MatTableDataSource(this.listCases);
          }

          if(!this.displayShowAll)
            this.caseDataSource = new MatTableDataSource(this.listCases);
               this.isLoading = false;
               this.caseDataSource.paginator = this.paginator;
                  this.caseDataSource.sort = this.sort;
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

  filterByFilter(filterValue: string) {
    if (filterValue == "open")
      filterValue = "true";
    if (filterValue == "closed")
      filterValue = "false";
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.caseDataSource.filter = filterValue;

  }

  filterByDate() {
    this.caseDataSource = new MatTableDataSource(
      this.listCases.filter(qcCase =>
        moment(qcCase.qcStartDate).isBetween(this.startDate, this.endDate, 'day', '[]')
      )
    );
  }
  onFilterKeyClose(event: KeyboardEvent) {
    this.showAllCases();
 }
}

export class SlideShowData {
  defectID: number;
  remarks: string; keys
  qcOpenPhotos: QCPhoto[];
  qcClosePhotos: QCPhoto[];
}

