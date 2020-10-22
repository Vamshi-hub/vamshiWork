import { Component, OnInit,ViewChild, Input } from '@angular/core';
import { animate, state, style, transition, trigger } from '@angular/animations';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSort, MatPaginator, MatTableDataSource, MatSelectChange, MatTable } from '@angular/material';
import { ProjectMaster } from '../classes/project-master';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { MaterialSlideShowComponent } from './slide-show/slide-show.component';
import { QCPhoto } from '../classes/qc-photo';
import { JobTrackService } from '../../job-track/job-track.service';
import * as moment from 'moment';
import { Jobstarted } from '../../job-track/classes/jobstarted';


@Component({
  selector: 'app-material-qc',
  templateUrl: './material-qc.component.html',
  styleUrls: ['./material-qc.component.css'],
  animations: [
    trigger('detailExpand', [
      state('collapsed', style({ height: '0px', minHeight: '0', display: 'none' })),
      state('expanded', style({ height: '*' })),
      transition('expanded <=> collapsed', animate('225ms cubic-bezier(0.4, 0.0, 0.2, 1)')),
    ]),
  ],
})


export class MaterialQcComponent extends CommonLoadingComponent {
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  project: ProjectMaster;
  blocks = [];
  selectedBlock = '';
  readytoproject:number;
  liststrct:Jobstarted[];
  jobid:number;
  columnsToDisplay = ['status', 'markingNo','type','stageName','qcStartDate','qcEndDate','qcFailedBy'];
  dataSource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  QCFailedstrctDatasource:MatTableDataSource<Jobstarted>=new MatTableDataSource();
  expandedElement: Jobstarted[];
  isDefectLoading = false;
  displayedColumns = ['checklistStatus', 'countPhotos', 'checkListName', 'qcFailedBy'];
  lstqcDefect: Jobstarted[];
  qcPhoto: QCPhoto[];
  slideShowData: SlideShowData;
  filterValue = '';
  displayShowAll = false;
  startDate: moment.Moment;
  endDate: moment.Moment;
  minDate = new Date();
  
  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService,private jobtrackservice:JobTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.dataSource = new MatTableDataSource();
    console.log(this.dataSource);
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        await this.route.paramMap.subscribe((params: Params) => {
          this.jobid = params.get('qcids');
          this.filterValue = params.get('filter');
          if ((this.jobid != null) || this.filterValue)
          this.displayShowAll = true;
          this.getstrctjobs(project.id)
        });
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No selected project', 'OK');
      }
      this.isLoading = false;
    });
  }

 showAllCases() {
  this.jobtrackservice.getStructuraljobs(this.project.id).subscribe(
    data => {
              this.liststrct = new Array();
              this.lstqcDefect = data;
              var results :Jobstarted[];
              results=data;

            for(var i=0;i<=data.length-1;i++) {
        if (this.liststrct.length == 0 || this.liststrct.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType).length === 0) {
          var ss = data.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType)
          var aa: Jobstarted=new Jobstarted();
          aa.tradeName = data[i].tradeName;
          aa.markingNo = data[i].markingNo;
          aa.status = data[i].status.replace(/_/g, ' ');
          aa.id = data[i].id;
          aa.qcStartDate = ss[0].qcStartDate;
          aa.qcEndDate = ss[ss.length - 1].qcEndDate;
          aa.stageName = ss[0].stageName;
          aa.qcFailedBy = ss[ss.length - 1].qcFailedBy;
          aa.actualStartDate = data[i].actualStartDate;
          aa.actualEndDate = data[i].actualEndDate;
          aa.block = data[i].block;
          aa.level = data[i].level;
          aa.materialType = data[i].materialType;
          aa.type = data[i].type;
          this.liststrct.push(aa);
        }
        }
      this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct);
      this.QCFailedstrctDatasource.paginator = this.paginator;
      this.QCFailedstrctDatasource.sort = this.sort;
      this.displayShowAll = false;
      this.filterValue='';
      this.startDate = null;
      this.endDate = null;
    });
}
  onFilterKeyIn(event: KeyboardEvent) {
    this.filterValue = (<HTMLInputElement>event.target).value;
    if(this.filterValue==''){
      this.showAllCases();
    }
    this.QCFailedstrctDatasource.filterPredicate = (data: Jobstarted, filter: string) => {
      const filterArray = filter.split(' ');
      console.log(filterArray);
      let countMatch = 0;
      for (let entry of filterArray) {
        entry = entry.toLowerCase();
        if (data.status.toLowerCase().indexOf(entry) >= 0 ||
        data.qcFailedBy.toLowerCase().indexOf(entry) >= 0 ||
        data.markingNo.toLowerCase().indexOf(entry) >=0)
          countMatch++;

      }
      if (countMatch >= filterArray.length)
        return true;
      else
        return false;
    };
    this.filterQCCases();
  }
  
  filterQCCases() {
    if (this.startDate && this.endDate)
      this.filterByDate();

    if (this.filterValue)
      this.filterByFilter(this.filterValue);
    this.QCFailedstrctDatasource.filter = this.filterValue;
    this.QCFailedstrctDatasource.paginator = this.paginator;
    this.QCFailedstrctDatasource.sort = this.sort;
  }

  filterByFilter(filterValue: string) {
    console.log('filter method called')
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.QCFailedstrctDatasource.filter = filterValue;
  }

  filterByDate() {
    this.QCFailedstrctDatasource = new MatTableDataSource(
      this.liststrct.filter(qcCase =>
        moment(qcCase.qcStartDate).isBetween(this.startDate, this.endDate, 'day', '[]')
      )
    );
    this.displayShowAll = true;
  }

  getstrctjobs(projectID: number) {
    this.jobtrackservice.getStructuraljobs(projectID).subscribe(
      data => {
        this.liststrct = new Array();
        this.lstqcDefect = data;
        var results :Jobstarted[];
        results=data;
        for(var i=0;i<=data.length-1;i++) {

          if (this.liststrct.length == 0 || this.liststrct.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType).length === 0) {
            
            var ss = data.filter(s => s.id === data[i].id  && s.markingNo === data[i].markingNo && s.block === data[i].block && s.level === data[i].level && s.materialType === data[i].materialType)
            var aa: Jobstarted=new Jobstarted();
            aa.tradeName = data[i].tradeName;
            aa.markingNo = data[i].markingNo;
            aa.status = data[i].status.replace(/_/g, ' ');
            aa.id = data[i].id;
            aa.qcStartDate = ss[0].qcStartDate;
            aa.qcEndDate = ss[ss.length - 1].qcEndDate;
            aa.stageName = ss[0].stageName;
            aa.qcFailedBy = ss[ss.length - 1].qcFailedBy;
            aa.actualStartDate = data[i].actualStartDate;
            aa.actualEndDate = data[i].actualEndDate;
            aa.block = data[i].block;
            aa.level = data[i].level;
            aa.type = data[i].type;
            this.liststrct.push(aa);
          }
          }
          if(this.filterValue){
            this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct.filter(s=>s.status==this.filterValue));
          }
          else if(this.jobid != null)
          {
           this.liststrct=this.liststrct.filter(s=>s.id==this.jobid)
           this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct);
           this.displayShowAll=true;
          }
          else{
            this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct);
          }
         
            if(!this.displayShowAll)
            this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct);
               this.isLoading = false;
               this.QCFailedstrctDatasource.paginator = this.paginator;
                  this.QCFailedstrctDatasource.sort = this.sort;
        },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
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
      this.getPhoto(row['checklistID'], this.project.id,0,row['id']);


  }
  selectJobRow(row: any) {
    this.dataSource = null;
    if (row == this.expandedElement) {
      this.expandedElement = Array<Jobstarted>();
      return false;
    }
    else {
      this.isDefectLoading = true;
      this.getDefectsByCase(row['id'], this.project.id);
      return row;
    }
  }

  getDefectsByCase(case_id: number, projectId: number) {
    this.dataSource = new MatTableDataSource(this.lstqcDefect.filter(d => d.id == case_id));
    if (this.dataSource.data.length > 0) {
      this.dataSource.data.forEach(item => {
        item.checklistStatus = item.checklistStatus.replace(/_/g, ' ');
      })
    }
    this.isDefectLoading = false;
    this.isLoading = false;
  }
    getPhoto(checklistID: number, projectID: number,MaterialStageAuditID:number,jobscheduleID:number) {
    this.isLoading = true;
      this.jobtrackservice.getQCPhotos(projectID, checklistID,MaterialStageAuditID,jobscheduleID).subscribe(data => {
      this.qcPhoto = data;
      if (this.qcPhoto != null) {
        this.slideShowData = new SlideShowData();
        this.slideShowData.qcOpenPhotos = this.qcPhoto;
        this.isLoading = false;
        this.uiUtilService.openDialog(MaterialSlideShowComponent, this.slideShowData, true);
      }
    })
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
