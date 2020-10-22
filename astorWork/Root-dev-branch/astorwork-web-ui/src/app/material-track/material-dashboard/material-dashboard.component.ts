import { Component, OnInit, ViewChild, ViewChildren, QueryList } from '@angular/core';
import { MaterialTrackService } from '../material-track.service';
import { Chart } from 'chart.js';

import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSort, MatPaginator, MatTableDataSource, MatSelectChange, matTabsAnimations } from '@angular/material';

import { UiUtilsService } from '../../shared/ui-utils.service';
import { ProjectMaster } from '../classes/project-master';
import { ProjectStats } from '../classes/Project-Stats';
import { OverallAndInProgress } from '../classes/overallAndInProgress';
import { OverallProgress } from '../classes/overallProgress';
import { InProgress } from '../classes/inProgress';
import { QcOpenMaterial } from '../classes/qcOpenMaterial';
import { DailyStatus } from '../classes/dailyStatus';
import * as moment from 'moment';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { Mrf } from '../classes/mrf-master';
import { MaterialMaster, MaterialMasterLHL } from '../classes/material-master';
import { Jobstarted } from '../../job-track/classes/jobstarted';
import { JobTrackService } from '../../job-track/job-track.service';
import { FormControl } from '@angular/forms';
@Component({
  selector: 'app-material-dashboard',
  templateUrl: './material-dashboard.component.html',
  styleUrls: ['./material-dashboard.component.css']
})
export class MaterialDashboardComponent  extends CommonLoadingComponent {
  lineChart = [];
  doughnutChart = [];
  displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn'];
  delayedDeliveryDisplayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo', 'expectedDelivery'];
  qcDataSource: MatTableDataSource<QcOpenMaterial> = new MatTableDataSource();
  // Delayed materials
  delayedMaterialDisplayedColumns = ['markingNo', 'orderNo', 'plannedDate', 'actualDate'];
  delayedProductionDS: MatTableDataSource<MaterialMasterLHL> = new MatTableDataSource();
  delayedDeliveryDS: MatTableDataSource<MaterialMasterLHL> = new MatTableDataSource();
  delayedInstallationDS: MatTableDataSource<MaterialMasterLHL> = new MatTableDataSource();
  delayedMaterials: any;
  readytoproject:number;
  readytoprojectprogress:number;
  projectStats: ProjectStats;
  project: ProjectMaster;
  overallAndInProgress: OverallAndInProgress;
  overallProgress: OverallProgress[];
  dailyStatus: DailyStatus;
  inProgress: InProgress[];
  bimVideoUrl: String;
  blocks = [];
  selectedBlock = '';
  QcFailCount = 0;
  @ViewChild(MatSort) DelayedDeliverySort: MatSort;
  @ViewChild(MatSort) QCSort: MatSort;
  @ViewChild(MatSort) DeliveredSort: MatSort;
  @ViewChild(MatSort) InstalledSort: MatSort;
  @ViewChild(MatSort) MRFSort: MatSort;
  @ViewChildren(MatPaginator) paginators: QueryList<MatPaginator>;
  @ViewChild('tabGroup') tabGroup;
  selected = new FormControl(0);
  cardClicked = 'QCOpen';
  QCFailedDisplayedColumns = ['tradeName', 'markingNo','checkListName', 'subConName','qcFailedBy','qcStartDate'];
  QCFailedstrctDisplayedColumns = ['type','markingNo','checkListName','qcFailedBy','qcStartDate'];
  readytoprojectdisplayedColumns=['jobname','block','level','zone','materialType']
  QCFailedJobsDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  QCFailedstrctDatasource:MatTableDataSource<Jobstarted>=new MatTableDataSource();
  deliveredDataSource = new MatTableDataSource();
  readyToDeliveredDataSource = new MatTableDataSource();
  installedDataSource = new MatTableDataSource();
  mrfDataSource = new MatTableDataSource();
  readytoprojectsitedatasource=new MatTableDataSource();
  materialDataSource: MatTableDataSource<MaterialMaster> = new MatTableDataSource();
  noData: boolean;
  lstMaterials: MaterialMaster[];
  lstreadytoproject:any;
  listCases: Jobstarted[];
  liststrct:Jobstarted[];
  ToggleView = true;
  dayProgress: number[];
  allDates: string[];
  BarChart=[]; 
  labels = [];
  counts = [];
  colors = [];
  
  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService,private jobtrackservice:JobTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.blocks = this.project.blocks;
        if (this.blocks) {
          this.selectedBlock = this.blocks[0];
          this.readytoproject=0;
        
          this.getProjectStatsById();
          this.getMaterialStageCounts();
        }       
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No selected project', 'OK');
      }
      this.isLoading = false;
    });
  }

  selectRow(row: any) {
    if (this.cardClicked == 'QCOpen') {
      let title = `${row.block}, Level ${row.level}, Zone ${row.zone}`;
      this.router.navigate(['material-tracking', 'qc-defects', {qcids: row['caseID'], caseName: row.caseName, title: title }]);
    }
    else if (this.cardClicked == 'InstalledMaterials' || this.cardClicked == 'DeliveredMaterials') {
      this.router.navigate(['material-tracking', 'materials', row['id']]);
    }
    else if (this.cardClicked == 'CompletedMRF') {
      this.router.navigate(['material-tracking', 'materials', { blk: row.block, mrfNo: row.mrfNo }]);
    }
    else if(this.cardClicked == 'ReadyToDeliveredMaterials'){
      this.router.navigate(['material-tracking', 'materials', row['id']]);
    }
    else if(this.cardClicked == 'ReadytoProjectsite'){
      this.router.navigate(['job-tracking', 'job-qc', {qcids: row['id'] }]);
    }
  }

  selectDelayedRow(row: any) {
    this.router.navigate(['material-tracking', 'materials', row['id']]);
  }

  filterAndOrderInProgress() {
    if (this.inProgress)
      return this.inProgress.filter(ip => ip.block == this.selectedBlock).slice(0, 3);
    else
      return [];
  }

  onBlockChanged(event: MatSelectChange) {
  }

  onDeliveredClick($element) {
    this.cardClicked = 'DeliveredMaterials';
    this.displayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo'];
    if (this.deliveredDataSource.data.length > 0) {
      this.deliveredDataSource.sort = this.DeliveredSort;
    }
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }

  onReadyToDeliverClick($element) {
    this.cardClicked = 'ReadyToDeliveredMaterials';
    this.displayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo'];
    if (this.readyToDeliveredDataSource.data.length > 0) {
      this.readyToDeliveredDataSource.sort = this.DeliveredSort;
    }
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }

  onInstalledClick($element) {
    this.cardClicked = 'InstalledMaterials';
    this.displayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo'];
    if (this.installedDataSource.data.length > 0) {
      this.installedDataSource.sort = this.InstalledSort;
    }
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }

  onQCOpenClick($element) {
    
    
  this.cardClicked = 'QCOpen';
  this.displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn'];
  if (this.qcDataSource.data.length > 0) {
    this.qcDataSource.sort = this.QCSort;
  }
  $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
}

  onCompletedMRFClick($element) {
    this.cardClicked = 'CompletedMRF';
    this.displayedColumns = ['mrfNo', 'block', 'level', 'zone', 'type'];
    if (this.mrfDataSource.data.length > 0) {
      this.mrfDataSource.sort = this.MRFSort;
    }
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }
  onreadytoprojectsiteClick($element){
    this.cardClicked = 'ReadytoProjectsite';
 this.readytoprojectdisplayedColumns=['jobname','block','level','zone','materialType']
    // if (this.readytoprojectsitedatasource.data.length > 0) {
    //   this.readytoprojectsitedatasource.sort = this.MRFSort;
    // }
    $element.scrollIntoView({behavior: "smooth", block: "start", inline: "nearest"});
  }
  getProjectStatsById() {
    this.getProjectProgress();
    this.getQcOpenMaterials();
    this.getReadytoproject()
    this.getarchiqcjobschecklists(this.project.id);
    this.getStructuraljobs(this.project.id);
    this.getReadyToDeliveredMaterials();
    this.materialTrackService.getProjectStats(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.projectStats = data;
        if(this.readytoproject > 0){
          this.readytoprojectprogress=parseFloat(((this.readytoproject/this.projectStats.requestedMaterialsCount)*100).toFixed(2));
        }
        
        else
        this.readytoprojectprogress=0;
        this.QcFailCount =this.QcFailCount + this.projectStats.qcFailedCount;
        // this.projectStats.qcFailedCount = this.QcFailCount;
      console.log('qc failed count' + this.QcFailCount);
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
    //this.getMaterials('All');
    this.getDelayedMaterials();
  }

  getProjectProgress() {
    this.materialTrackService.getPrjectProgress(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.bimVideoUrl = data["bimVideoUrl"];
        this.inProgress = data["inProgress"];
        this.overallProgress = data["overallProgress"];
        //this.overallProgress = this.overallProgress.filter(op => moment().diff(moment(op.date), 'days') <= 7);
        //#region chart data
        this.dayProgress = this.overallProgress.map(res => res.progress);
        this.allDates = this.overallProgress.map(res => moment(res.date).format('DD/MM/YYYY'));
        this.loadLineChart();
        //#endregion
      },
      error => {
        console.log(error);
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }
getReadyToDeliveredMaterials() {
    this.materialTrackService.getReadyToDeliveredMaterialsByProjectId(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.readyToDeliveredDataSource = new MatTableDataSource(data["readyToDeliveredMaterials"])
        if (this.qcDataSource.data.length > 0) {
          this.qcDataSource.sort = this.QCSort;
        }
      },
      error => {
        console.log(error);
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getQcOpenMaterials() {
    this.materialTrackService.getQcOpenMaterialsByProjectId(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.qcDataSource = new MatTableDataSource(data["qcOpenMaterials"])
        this.deliveredDataSource = new MatTableDataSource(data["deliveredMaterials"])
        this.installedDataSource = new MatTableDataSource(data["installedMaterials"])
        this.mrfDataSource = new MatTableDataSource(data["completedMRFs"])

        if (this.qcDataSource.data.length > 0) {
          this.qcDataSource.sort = this.QCSort;
        }

        this.dailyStatus = data["dailyMaterialStatusCount"]
        let deliveredCount = this.dailyStatus.deliveredCount;
        let installedCount = this.dailyStatus.installedCount;
        let producedCount = this.dailyStatus.producedCount;
        console.log(this.dailyStatus)
        if (this.dailyStatus.deliveredCount == 0 && this.dailyStatus.installedCount == 0 && this.dailyStatus.producedCount == 0)
          this.noData = true;
        this.doughnutChart = new Chart('canDoughnutChart', {
          type: 'doughnut',
          data: {
            labels: ["Received", "Installed", "Production",],
            datasets: [
              {
                label: "Poputation (millions)",
                backgroundColor: ["#66ff99", "#00dbff", "#ffff66"],
                data: [deliveredCount, installedCount, producedCount]
              }
            ]
          },
          options: {
            cutoutPercentage: 60
          }
        });
      },
      error => {
        console.log(error);
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getDelayedMaterials() {
    this.materialTrackService.getDelayedMaterials(this.project.id).subscribe(
      data => {
        this.delayedMaterials = data;
        this.isLoading = false;
        let listPaginators = this.paginators.toArray();
        this.delayedProductionDS = new MatTableDataSource(data["listDelayedProduction"]);
        this.delayedProductionDS.paginator = listPaginators[0];
        this.delayedDeliveryDS = new MatTableDataSource(data["listDelayedDelivery"]);
        this.delayedDeliveryDS.paginator = listPaginators[1];
        this.delayedInstallationDS = new MatTableDataSource(data["listDelayedInstallation"]);
        this.delayedInstallationDS.paginator = listPaginators[2];
      },
      error => {
        console.log(error);
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );

  }

  getMaterials(blk: string) {
    this.materialTrackService.getMaterials(this.project.id, blk).subscribe(
      data => {
        this.lstMaterials = data;
        this.materialDataSource = new MatTableDataSource(this.lstMaterials
          .filter(
            m => m.expectedDeliveryDate != null && m.stageOrder < this.lstMaterials[0].deliveryStageOrder && moment(m.expectedDeliveryDate) < moment().startOf('day')
          ).slice(0, 10)
        );
        if (this.materialDataSource.data.length > 0) {
          this.materialDataSource.sort = this.DelayedDeliverySort;
        }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
getReadytoproject(){
  this.materialTrackService.getreadytoprojectstats(this.project.id).subscribe(data =>{
    //this.readytoproject=data.readytoProject;
    this.lstreadytoproject=data;
    console.log(this.lstreadytoproject)
    this.readytoprojectsitedatasource=this.lstreadytoproject;
    this.readytoproject=this.lstreadytoproject.length;
  },
  error => {
    this.isLoading = false;
    this.uiUtilService.openSnackBar(error, 'OK');
  });
}
getarchiqcjobschecklists(projectID: number) {
  this.jobtrackservice.getarchiqcjobschecklists(projectID).subscribe(
    data => {
      this.listCases = data;
     
      this.listCases = this.listCases.slice().sort();
      var results = [];
      for (var i = 0; i < this.listCases.length - 1; i++) {
        if (this.listCases[i + 1].markingNo === this.listCases[i].markingNo && this.listCases[i + 1].tradeName === this.listCases[i].tradeName)
          results.push(this.listCases[i]);
      }
      results.forEach((element:any) => {
        let temp = element;
        let index = this.listCases.indexOf(temp);
        this.listCases.splice(index, 1);
        let indexmarkingNo = this.listCases.findIndex(j => j.markingNo === temp.markingNo);
        if (new Date(temp.qcStartDate) > new Date(this.listCases[indexmarkingNo].qcStartDate)) {
          this.listCases.push(temp);
        } else {
          this.listCases.push(this.listCases[indexmarkingNo])
        }
        this.listCases.splice(indexmarkingNo, 1);
      });
      this.QCFailedJobsDatasource = new MatTableDataSource(this.listCases.filter(s=>s.status=='QC_failed_by_Maincon'));
      this.isLoading = false;
      this.QcFailCount =this.QcFailCount + this.QCFailedJobsDatasource.data.length;
      console.log('qc failed count' + this.QcFailCount);
    },
    error => {
      this.isLoading = false;
      this.uiUtilService.openSnackBar(error, "OK");
    }
  );
}
getStructuraljobs(projectID: number) {
  this.jobtrackservice.getStructuraljobs(projectID).subscribe(
    data => {
      this.liststrct = data;
      this.liststrct = this.liststrct.slice().sort();
      // var results = [];
      // for (var i = 0; i < this.liststrct.length - 1; i++) {
      //   if (this.liststrct[i + 1].markingNo === this.liststrct[i].markingNo && this.liststrct[i + 1].tradeName === this.liststrct[i].tradeName) {
      //     results.push(this.liststrct[i]);
      //   }
      // }
      // results.forEach((element:any) => {
      //   let temp = element;
      //   let index = this.liststrct.indexOf(temp);
      //   this.liststrct.splice(index, 1);
      //   let indexmarkingNo = this.liststrct.findIndex(j => j.markingNo === temp.markingNo);
      //   if (new Date(temp.qcStartDate) > new Date(this.liststrct[indexmarkingNo].qcStartDate)) {
      //     this.liststrct.push(temp);
      //   } else {
      //     this.liststrct.push(this.liststrct[indexmarkingNo])
      //   }
      //   this.liststrct.splice(indexmarkingNo, 1);
      // });
      this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct.filter(s=>s.status=='QC_failed_by_Maincon'));
      this.isLoading = false;
      this.QcFailCount =this.QcFailCount + this.QCFailedstrctDatasource.data.length;
    },
    error => {
      this.isLoading = false;
      this.uiUtilService.openSnackBar(error, "OK");
    }
  );
}

  toggleDesign() {
    if (this.ToggleView)
      this.ToggleView = false
    else
      this.ToggleView = true;
  }
  selectArchiRow(row: any){
    this.router.navigate(['job-tracking', 'job-qc', {qcids: row['id'] }]);
  }

  selectStructuralRow(row: any){
    this.router.navigate(['material-tracking', 'material-qc', {qcids: row['id'] }]);
  }
 
  getMaterialStageCounts() {
    this.materialTrackService.getMaterialStageCounts(this.project.id).subscribe(
      data => {
        data.forEach(item => {
          this.labels.push(item.name);
          this.counts.push(item.materialCount);
          this.colors.push(item.colour);
        })
        this.loadBarChart();
        this.labels = [];
        this.counts = [];
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
    
  }

  tabClick(tab){
    if (tab.tab.textLabel=="1"){
      this.loadLineChart();
    }
    else if (tab.tab.textLabel=="3"){
      this.getMaterialStageCounts();
    }
  }
  
  loadBarChart(){
    this.BarChart = new Chart('barchart', {  
      type: 'bar',  
      data: {  
        labels: this.labels,  
        datasets: [  
          {  
           data: this.counts,
            borderColor: '#3cb371',  
            backgroundColor: this.colors 
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
  }

  loadLineChart(){
    this.lineChart = new Chart('canLineChart', {
      type: 'line',
      data: {
        labels: this.allDates,
        datasets: [
          {
            data: this.dayProgress,
            borderColor: '#ffcc00',
            fill: false
          },
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
  }
}
