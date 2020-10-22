import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { Jobstarted } from '../classes/jobstarted';
import { DatePipe } from '@angular/common';
import { JobStatus } from '../classes/job-status';
import { ProjectMaster } from '../classes/project-master';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { JobTrackService } from '../job-track.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { FormBuilder } from '@angular/forms';
import { Chart } from 'chart.js';  
import { DailyStatus } from '../classes/dailyStatus';
import { OverallProgress } from '../../material-track/classes/overallProgress';
import * as moment from 'moment';
import * as ChartLabel from 'chartjs-plugin-datalabels';
import { Moment } from 'moment';

@Component({
  selector: 'app-job-dashboard',
  templateUrl: './job-dashboard.component.html',
  styleUrls: ['./job-dashboard.component.css']
})

export class JobDashboardComponent extends CommonLoadingComponent {
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  DelayedDisplayedColumns = ['tradeName', 'markingNo', 'subConName', 'plannedStartDate'];
  StartedDispalyedColumns = ['tradeName', 'markingNo', 'subConName', 'plannedStartDate','actualStartDate'];
  CompletdDisplayedcolumns= ['tradeName', 'markingNo', 'subConName', 'actualStartDate','actualEndDate'];
  QCFailedDisplayedColumns = ['tradeName', 'markingNo','checkListName', 'subConName','qcFailedBy','qcStartDate'];
  doughnutChart =[];
  Linechart = []; 
  BarChart=[]; 
  QCFailedJobsCount:number;
  delayedJobsProgress :number;
  startedJobsProgress :number;
  compltedJobsProgress :number;
  qcFailedJobsProgress=0;
  TotalQccount=0;
  projectname:string;
  noData:boolean;
  listCases: Jobstarted[];
  dailyStatus: DailyStatus
  cardClicked = 'QcFailed';
  OverallProgress:OverallProgress[];
  StartedJobsDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  CompletedJobsDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  DelayedJobsDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  QCFailedJobsDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  JobStatus:JobStatus
  renderedData:any;
  project: ProjectMaster;
  startDate: Moment;
  endDate: Moment;
  projectManagerName: string;
  blocks = [];
  selectedBlock = '';
  ToggleView = true;
  @ViewChild(MatSort) StartedJobsort: MatSort;
  @ViewChild(MatSort) compledtedJobsort: MatSort;
  @ViewChild(MatSort) DelayedJobsort: MatSort;
  @ViewChild(MatSort) QcfailedJobsort: MatSort;
  constructor(route: ActivatedRoute, private fb: FormBuilder, router: Router, private uiUtilService: UiUtilsService, private jobtrackservice: JobTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit()
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      
      if (project) {
        this.project = project;
        this.startDate=this.project.startDate;
     this.endDate=this.project.endDate;
    this.projectManagerName=this.project.projectManagerName;
        this.blocks = this.project.blocks;
        if (this.blocks) {
          this.selectedBlock = this.blocks[0];
          this.getarchiqcjobschecklists(this.project.id);
          this.getJobslist() ;
          this.getDailyJobStatus();
          this.getJobProgress();
          this.getOverallJobProgress();  
          Chart.plugins.register(ChartLabel);       
        }
      }
      else {
        this.isLoading=false;
        this.uiUtilService.openSnackBar('No selected project', 'OK');
      }
      this.isLoading = false;
    });
  }
  getarchiqcjobschecklists(projectID: number) {
    this.jobtrackservice.getarchiqcjobschecklists(projectID).subscribe(
      data => {
        this.listCases = data;
       
        this.listCases = this.listCases.slice().sort();
        var results = [];
        for (var i = 0; i < this.listCases.length - 1; i++) {
          if (this.listCases[i + 1].markingNo === this.listCases[i].markingNo && this.listCases[i + 1].tradeName === this.listCases[i].tradeName) {
            results.push(this.listCases[i]);
          }
        }
        results.forEach((element:any) => {
          let temp = element;
          let index = this.listCases.indexOf(temp);
          this.listCases.splice(index, 1);
          let indexmarkinNo = this.listCases.findIndex(j => j.markingNo === temp.markingNo);
          if (new Date(temp.qcStartDate) > new Date(this.listCases[indexmarkinNo].qcStartDate)) {
            this.listCases.push(temp);
          } else {
            this.listCases.push(this.listCases[indexmarkinNo])
          }
          this.listCases.splice(indexmarkinNo, 1);
        });
        this.QCFailedJobsDatasource = new MatTableDataSource(this.listCases.filter(s=>s.status=='QC_failed_by_Maincon'));
        this.isLoading = false;
        this.QCFailedJobsCount=this.listCases.filter(s=>s.status=='QC_failed_by_Maincon').length;
        if(this.listCases.length>0)
        this.TotalQccount= this.listCases.length;//getting TotalQc count record at 0th level 
      if(this.QCFailedJobsCount!=0)
      {
        this.qcFailedJobsProgress=parseFloat(((this.QCFailedJobsCount/this.TotalQccount)*100).toFixed(2));
      }
      else{
        this.qcFailedJobsProgress=0;
      }
  
        this.QCFailedJobsDatasource.paginator = this.paginator;
       this.QCFailedJobsDatasource.sort = this.sort;
       
      
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  getJobslist() {
    this.jobtrackservice.getJobList(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.StartedJobsDatasource = new MatTableDataSource(data["startedJobs"]);
        this.CompletedJobsDatasource = new MatTableDataSource(data["compltedJobs"]);
        this.DelayedJobsDatasource = new MatTableDataSource(data["delayedJobs"]);
          },
      error => {
        console.log(error);
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }
  selectRow(row: any) {
    if (this.cardClicked == 'QcFailed') {
      this.router.navigate(['job-tracking', 'job-qc', {qcids: row['id'] }]);
    }
    else if (this.cardClicked == 'NotStartedJob') {
      this.router.navigate(['job-tracking', 'job-scheduling',{qcids: row['id']}]);
    }
    else if (this.cardClicked == 'Started') {
      this.router.navigate(['job-tracking', 'job-scheduling',{qcids: row['id']}]);
    }
    else if (this.cardClicked == 'Completed') {
      this.router.navigate(['job-tracking', 'job-scheduling',{qcids: row['id']}]);
    }

  }


  OnNotstartedJobsclick($element){
    this.cardClicked = 'NotStartedJob';
    this.DelayedDisplayedColumns = ['tradeName', 'markingNo', 'subConName', 'plannedStartDate'];
    if (this.DelayedJobsDatasource.data.length > 0) {
      setTimeout(() => {
        this.DelayedJobsDatasource.sort = this.DelayedJobsort;
      },);
     
    }
  
    
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }
  OnstartedClick($element){
    this.cardClicked = 'Started';
    this.StartedDispalyedColumns = ['tradeName', 'markingNo', 'subConName', 'plannedStartDate','actualStartDate'];
     
      if (this.StartedJobsDatasource.data.length > 0) {
        setTimeout(() => {
          this.StartedJobsDatasource.sort = this.StartedJobsort;
        },);
       
      }
    
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }
  OnCompletedClick($element){
    this.cardClicked = 'Completed';
    this.CompletdDisplayedcolumns= ['tradeName', 'markingNo', 'subConName', 'actualStartDate','actualEndDate'];
    if (this.CompletedJobsDatasource.data.length > 0) {
      setTimeout(() => {
        this.CompletedJobsDatasource.sort = this.compledtedJobsort;
      },);
    
    }
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }

  OnQCFailedClick($element){
    this.cardClicked = 'QcFailed';
    this. QCFailedDisplayedColumns = ['tradeName', 'markingNo','checkListName', 'subConName','qcFailedBy','qcStartDate'];
    if (this.QCFailedJobsDatasource.data.length > 0) {
      setTimeout(() =>{
        this.QCFailedJobsDatasource.sort = this.QcfailedJobsort;
      },);
    }
  
  $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }


  getDailyJobStatus() {
    this.jobtrackservice.getDailyJobProgress(this.project.id).subscribe(
      data => {
        this.dailyStatus = data;
        console.log(this.dailyStatus)
        let ongoingCount = this.dailyStatus.ongoingCount;
         let completedCount = this.dailyStatus.completedCount;
         let scheduledCount = this.dailyStatus.scheduledCount;
         if (this.dailyStatus.ongoingCount == 0 && this.dailyStatus.completedCount == 0 && this.dailyStatus.scheduledCount == 0)
         this.noData = true;
        this.doughnutChart = new Chart('canDoughnutChart', {
          type: 'doughnut',
          data: {
            labels:["Scheduled","Ongoing","Completed"],
            datasets: [
              { 
                  
                    backgroundColor: ["#66ff99", "#ff9b06", "#3399ff"],
                    data: [scheduledCount,ongoingCount,completedCount]
              },
            ]
          },
          options: {
            
            legend: {
              display: true
            },
            tooltips:{
              enabled:true
            }
           },
          plugins: {
            // Change options for ALL labels of THIS CHART
            datalabels: {
              color: 'black',
              align: 'top',
            }
          }
        });
      },
      error => { 
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
        
      }
    );
  }
  
  getOverallJobProgress() {
    this.jobtrackservice.getOverallJobProgress(this.project.id).subscribe(
      data=>{
        this.OverallProgress = data["overallProgress"];
        let dayProgress = this.OverallProgress.map(res => res.progress)
        let alldates = this.OverallProgress.map(res => moment(res.date).format('DD/MM/YYYY'))
        this.Linechart = new Chart('linechart', {  
          type: 'line',  
          data: {  
            labels: alldates,    
            datasets: [  
              {  
                data: dayProgress,
                borderColor: '#ffcc00',
                fill: false
              }  
            ]  
          },  
          options: {
          
            legend: {  
              display: false  
            },  
            scales: {
              xAxes: [{
                scaleLabel: {
                  display: true,
                  labelString: "Date",
                },
              }],
              yAxes: [{
                scaleLabel: {
                  display: true,
                  labelString: "Progress in %",
                },
                ticks: {
                  beginAtZero: true
                }
              }
              ],
            }  
          }  
        });
      })
 
  }

  getJobProgress() {
    this.jobtrackservice.getJobStatus(this.project.id).subscribe(
      data => {
        this.JobStatus = data;
        let scheduledJobsCount = this.JobStatus.scheduledJobsCount;
         let delayedJobsCount = this.JobStatus.delayedJobsCount;
         let startedJobsCount = this.JobStatus.startedJobsCount;
         let completedJobsCount = this.JobStatus.completedJobsCount;
         let QCFailedJobsCount = this.QCFailedJobsCount;
         if((this.JobStatus.scheduledJobsCount || this.JobStatus.delayedJobsCount||this.JobStatus.completedJobsCount
          || this.JobStatus.totalQCCount) !=0){
           this.delayedJobsProgress=parseFloat(((this.JobStatus.delayedJobsCount/this.JobStatus.scheduledJobsCount)*100).toFixed(2));
           this.compltedJobsProgress=parseFloat(((this.JobStatus.completedJobsCount/this.JobStatus.scheduledJobsCount)*100).toFixed(2));
           this. startedJobsProgress=parseFloat(((this.JobStatus.startedJobsCount/this.JobStatus.scheduledJobsCount)*100).toFixed(2));
          }
          else{
           this. delayedJobsProgress=0
           this. compltedJobsProgress=0
           this. startedJobsProgress=0
          }
          if(this.TotalQccount==0)
          this.TotalQccount=0;
         this.BarChart = new Chart('barchart', {  
          type: 'bar',  
          data: {  
            labels: ["Scheduled","Delayed", "Ongoing", "Completed", "QC Failed"],  
            datasets: [  
              {  
               data: [scheduledJobsCount,delayedJobsCount, startedJobsCount,completedJobsCount,QCFailedJobsCount],
                borderColor: '#3cb371',  
                backgroundColor: ["#5ae288","#cea051","#ff9b06","#3399ff","#fb4a46"] 
              }  
            ]  
          },  
          options: {
            legend: {  
              display: false  
            }, 
            plugins: {
              // Change options for ALL labels of THIS CHART
              datalabels: {
                color: 'black',
                align: 'top',
              }
            }, 
            scales: {  
              xAxes: [{ 
                scaleLabel: { 
                display: true,
                labelString: "Status" 
                } 
              }],  
              yAxes: [{ 
                scaleLabel: { 
                display: true,
                labelString: "Count",
                },
                ticks: {
                  beginAtZero: true
                } 
              }],  
            }  
          }  
        });
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
        
      }
    );
  }
  toggleDesign() {
    if (this.ToggleView)
      this.ToggleView = false
    else
      this.ToggleView = true;
  }
}
