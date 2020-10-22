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
import { MaterialMaster, MaterialMasterLHL } from '../classes/material-master';
import { Jobstarted } from '../../job-track/classes/jobstarted';
import { JobTrackService } from '../../job-track/job-track.service';
import { ConfigurationService } from '../../configuration/configuration.service';
import { StageMaster } from '../../material-track/classes/stage-master';
import { MaterialStageDetail } from '../classes/material-stage-detail';
import { Moment } from 'moment';

@Component({
  selector: 'app-materialstagedashboard',
  templateUrl: './materialstagedashboard.component.html',
  styleUrls: ['./materialstagedashboard.component.css']
})
export class MaterialstagedashboardComponent extends CommonLoadingComponent {
  stages: StageMaster[];
  materialstatges: MaterialStageDetail[];
  lineChart = [];
  doughnutChart: any;
  BarChart = [];
  labels = [];
  counts = [];
  colors = [];
  displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn'];
  qcDataSource: MatTableDataSource<QcOpenMaterial> = new MatTableDataSource();
  projectStats: ProjectStats;
  project: ProjectMaster;
  overallAndInProgress: OverallAndInProgress;
  overallProgress: OverallProgress[];
  dailyStatus: DailyStatus;
  inProgress: InProgress[];
  bimVideoUrl: String;
  blocks = [];
  selectedBlock = '';
  @ViewChild(MatSort) QCSort: MatSort;
  @ViewChildren(MatPaginator) paginators: QueryList<MatPaginator>;
  cardClicked = 'QCOpen';
  QCFailedDisplayedColumns = ['tradeName', 'markingNo', 'checkListName', 'subConName', 'qcFailedBy', 'qcStartDate'];
  QCFailedstrctDisplayedColumns = ['type', 'markingNo', 'checkListName', 'qcFailedBy', 'qcStartDate'];
  QCFailedJobsDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  QCFailedstrctDatasource: MatTableDataSource<Jobstarted> = new MatTableDataSource();
  noData: boolean;
  listCases: Jobstarted[];
  liststrct: Jobstarted[];
  tile1stageid: number;
  tile2stageid: number
  tile3stageid: number
  ToggleView = true;
  installedCount: number;
  producedcount: number;
  tile1color: string;
  tile2color: string;
  tile3color: string;
  tile1stagename: string;
  tile2stagename: string;
  tile3stagename: string;
  tile1MaterialCount: number;
  tile2MaterialCount: number;
  tile3MaterialCount: number;
  tile1Materialcountdaily: number;
  tile2Materialcountdaily: number;
  tile3Materialcountdaily: number;
  tile1MaterialDatasource = new MatTableDataSource();
  tile2MaterialDatasource = new MatTableDataSource();
  tile3MaterialDatasource = new MatTableDataSource();
  istile1click = false;
  istile2click = false;
  istile3click = false;
  tiledisplayedcolumns = ['block', 'level', 'zone', 'materialType', 'mrf', 'qcstatus'];
  tilemateriallist: any;
  tile1materilaprogress: number;
  tile2materilaprogress: number;
  tile3materilaprogress: number;
  RequestedMaterialsCount: number
  TotalMaterialsCount: number;
  satgeorder: number;
  status: string;
  startdate: Moment;
  enddate: Moment;
  QcFailCount = 0;
  tile1QCcount: number
  tile2QCcount: number
  tile3QCcount: number
  projectManagerName: string;
  ischecked1 = false;
  ischecked2 = false;
  ischecked3 = false;
  istile1qccount = false;
  istile3qccount = false;
  istile2qccount = false;
  tile1noqccount: number;
  tile2noqccount: number;
  tile3noqccount: number;
  @ViewChild(MatPaginator) paginator: MatPaginator
  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService, private jobtrackservice: JobTrackService, private configService: ConfigurationService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.getStages();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.blocks = this.project.blocks;
        this.startdate = this.project.startDate;
        this.enddate = this.project.endDate;
        this.projectManagerName = this.project.projectManagerName;
        if (this.blocks) {
          this.selectedBlock = this.blocks[0];
          this.getProjectStatsById();
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
      this.router.navigate(['material-tracking', 'qc-defects', { qcids: row['caseID'], caseName: row.caseName, title: title }]);
    }
    else {
      this.router.navigate(['material-tracking', 'materials', row['id']]);
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

  ontile1click($element) {
    this.istile1click = true;
    this.istile2click = false;
    this.istile3click = false;
    this.cardClicked = this.tile1stagename + 'Materials';
    this.tiledisplayedcolumns = ['block', 'level', 'zone', 'markingNo', 'type', 'mrfNo', 'qcstatus'];
    this.satgeorder = this.stages.find(s => s.name.trim() == this.tile1stagename.trim()).order;
    if (this.tile1MaterialDatasource.data.length > 0) {
      setTimeout(() => {
        this.tile1MaterialDatasource.paginator = this.paginator;
      });

    }
    $element.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });
  }
  ontile2click($element) {
    this.istile2click = true;
    this.istile1click = false;
    this.istile3click = false;
    this.cardClicked = this.tile2stagename + 'Materials';
    this.tiledisplayedcolumns = ['block', 'level', 'zone', 'markingNo', 'type', 'mrfNo', 'qcstatus'];
    this.satgeorder = this.stages.find(s => s.name.trim() == this.tile2stagename.trim()).order;
    if (this.tile2MaterialDatasource.data.length > 0) {
      setTimeout(() => {
        this.tile2MaterialDatasource.paginator = this.paginator;
      });

    }
    $element.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });

  }
  ontile3click($element) {
    this.istile3click = true;
    this.istile1click = false;
    this.istile2click = false;
    this.cardClicked = this.tile3stagename + 'Materials';
    this.tiledisplayedcolumns = ['block', 'level', 'zone', 'markingNo', 'type', 'mrfNo', 'qcstatus'];
    this.satgeorder = this.stages.find(s => s.name.trim() == this.tile3stagename.trim()).order;
    if (this.tile3MaterialDatasource.data.length > 0) {
      setTimeout(() => {
        this.tile3MaterialDatasource.paginator = this.paginator;
      });

    }
    $element.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });

  }

  onQCOpenClick($element) {
    this.cardClicked = 'QCOpen';
    this.displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn'];
    if (this.qcDataSource.data.length > 0) {
      this.qcDataSource.sort = this.QCSort;
    }
    $element.scrollIntoView({ behavior: "smooth", block: "start", inline: "nearest" });
  }
  getProjectStatsById() {

    this.getProjectProgress();
    this.getQcOpenMaterials();
    this.getarchiqcjobschecklists(this.project.id);
    this.getStructuraljobs(this.project.id);
    this.getmaterialstagestatus(this.project.id);
    this.getQcDefects();
    this.materialTrackService.getProjectStats(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.projectStats = data;
        this.QcFailCount = this.QcFailCount + this.projectStats.qcFailedCount;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getProjectProgress() {
    this.materialTrackService.getPrjectProgress(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.bimVideoUrl = data["bimVideoUrl"];
        this.inProgress = data["inProgress"];
        this.overallProgress = data["overallProgress"];
        let dayProgress = this.overallProgress.map(res => res.progress)
        let alldates = this.overallProgress.map(res => moment(res.date).format('DD/MM/YYYY'))
        this.lineChart = new Chart('canLineChart', {
          type: 'line',
          data: {
            labels: alldates,
            datasets: [
              {
                data: dayProgress,
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
        //#endregion
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }
  loadBarChart() {
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
            },
            ticks: {
              autoSkip: false
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
  tabClick(tab) {
    if (tab.tab.textLabel == "3") {
      this.getMaterialStageCounts();
    }
  }
  getQcOpenMaterials() {
    if (this.tile1Materialcountdaily == 0 && this.tile2Materialcountdaily == 0 && this.tile3Materialcountdaily == 0)
      this.noData = true;
    this.doughnutChart = new Chart('canDoughnutChart', {
      type: 'doughnut',
      data: {
        labels: [this.tile1stagename, this.tile2stagename, this.tile3stagename],
        datasets: [
          {
            backgroundColor: [this.tile1color, this.tile2color, this.tile3color],
            data: [this.tile1Materialcountdaily, this.tile2Materialcountdaily, this.tile3Materialcountdaily],
          }
        ]
      },

      options: {
        cutoutPercentage: 60
      }
    }
    );
  }
  getQcDefects() {
    this.materialTrackService.getQcOpenMaterialsByProjectId(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.qcDataSource = new MatTableDataSource(data["qcOpenMaterials"])
        if (this.qcDataSource.data.length > 0) {
          this.qcDataSource.sort = this.QCSort;
        }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
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
        results.forEach((element: any) => {
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
        this.QCFailedJobsDatasource = new MatTableDataSource(this.listCases.filter(s => s.status == 'QC_failed_by_Maincon'));
        this.isLoading = false;
        this.QcFailCount = this.QcFailCount + this.QCFailedJobsDatasource.data.length;

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
        var results = [];
        for (var i = 0; i < this.liststrct.length - 1; i++) {
          if (this.liststrct[i + 1].markingNo === this.liststrct[i].markingNo && this.liststrct[i + 1].tradeName === this.liststrct[i].tradeName) {
            results.push(this.liststrct[i]);
          }
        }
        results.forEach((element: any) => {
          let temp = element;
          let index = this.liststrct.indexOf(temp);
          this.liststrct.splice(index, 1);
          let indexmarkingNo = this.liststrct.findIndex(j => j.markingNo === temp.markingNo);
          if (new Date(temp.qcStartDate) > new Date(this.liststrct[indexmarkingNo].qcStartDate)) {
            this.liststrct.push(temp);
          } else {
            this.liststrct.push(this.liststrct[indexmarkingNo])
          }
          this.liststrct.splice(indexmarkingNo, 1);
        });
        this.QCFailedstrctDatasource = new MatTableDataSource(this.liststrct.filter(s => s.status == 'QC_failed_by_Maincon'));
        this.isLoading = false;
        this.QcFailCount = this.QcFailCount + this.QCFailedstrctDatasource.data.length;

      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }

  changedtile1QC() {
    if (this.ischecked1 == true) {
      this.tile1QCcount = this.materialstatges.filter(s => s.stageID == this.tile1stageid && s.qCStatus == "All QC passed").length;
      this.tile1MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == this.tile1stageid));
    }
    else {
      this.ischecked1 = false;
      this.tile1MaterialCount = this.materialstatges.filter(s => s.stageID == this.tile1stageid).length;
      this.tile1MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == this.tile1stageid));
    }

  }
  changedtile2QC() {
    if (this.ischecked2 == true) {
      this.tile2QCcount = this.materialstatges.filter(s => s.stageID == this.tile2stageid && s.qCStatus == "All QC passed").length;
      this.tile2MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == this.tile2stageid));
    }
    else {
      this.tile2MaterialCount = this.materialstatges.filter(s => s.stageID == this.tile2stageid).length;
      this.tile2MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == this.tile2stageid));
    }

  }
  changedtile3QC() {
    if (this.ischecked3 == true) {
      this.tile3QCcount = this.materialstatges.filter(s => s.stageID == this.tile3stageid && s.qCStatus == "All QC passed").length;
      this.tile3MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == this.tile3stageid));
    }
    else {
      this.tile3MaterialCount = this.materialstatges.filter(s => s.stageID == this.tile3stageid).length;
      this.tile3MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == this.tile3stageid));
    }

  }
  toggleDesign() {
    if (this.ToggleView)
      this.ToggleView = false
    else
      this.ToggleView = true;
  }
  selectArchiRow(row: any) {
    this.router.navigate(['job-tracking', 'job-qc', { qcids: row['id'] }]);
  }

  selectStructuralRow(row: any) {
    this.router.navigate(['material-tracking', 'material-qc', { qcids: row['id'] }]);
  }

  getStages() {
    this.configService.getMaterialStages().subscribe(
      data => {
        this.stages = data;
        this.tile1stageid = data.find(s => s.milestoneID == 1).id;
        this.tile1color = data.find(s => s.milestoneID == 1).colour;
        this.tile1stagename = data.find(s => s.milestoneID == 1).name;
        this.tile2stageid = data.find(s => s.milestoneID == 2).id;
        this.tile2color = data.find(s => s.milestoneID == 2).colour;
        this.tile2stagename = data.find(s => s.milestoneID == 2).name;
        this.tile3stageid = data.find(s => s.milestoneID == 3).id;
        this.tile3color = data.find(s => s.milestoneID == 3).colour;
        this.tile3stagename = data.find(s => s.milestoneID == 3).name;
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
  getmaterialstagestatus(projectID: number) {
    this.materialTrackService.getMaterialStagestatus(projectID).subscribe(
      data => {
        this.materialstatges = data;
        this.tile2MaterialCount = data.filter(s => s.stageID == this.tile2stageid).length;
        this.tile1MaterialCount = data.filter(s => s.stageID == this.tile1stageid).length;
        this.tile3MaterialCount = data.filter(s => s.stageID == this.tile3stageid).length;
        this.tile1Materialcountdaily = data.filter(s => s.stageID == this.tile1stageid && s.createdDate == s.utcDate).length;
        this.tile2Materialcountdaily = data.filter(s => s.stageID == this.tile2stageid && s.createdDate == s.utcDate).length;
        this.tile3Materialcountdaily = data.filter(s => s.stageID == this.tile3stageid && s.createdDate == s.utcDate).length;
        this.TotalMaterialsCount = data[0].totalmaterialCount;
        this.tile1QCcount = this.materialstatges.filter(s => s.stageID == this.tile1stageid && s.qCStatus == "All QC passed").length;
        this.tile2QCcount = this.materialstatges.filter(s => s.stageID == this.tile2stageid && s.qCStatus == "All QC passed").length;
        this.tile3QCcount = this.materialstatges.filter(s => s.stageID == this.tile3stageid && s.qCStatus == "All QC passed").length;
        this.tile1noqccount = this.materialstatges.filter(s => s.stageID == this.tile1stageid && s.qCStatus == "NA").length;
        this.tile2noqccount = this.materialstatges.filter(s => s.stageID == this.tile2stageid && s.qCStatus == "NA").length;
        this.tile3noqccount = this.materialstatges.filter(s => s.stageID == this.tile3stageid && s.qCStatus == "NA").length;
        this.tile1MaterialDatasource = new MatTableDataSource(data.filter(s => s.stageID == this.tile1stageid));
        this.tile2MaterialDatasource = new MatTableDataSource(data.filter(s => s.stageID == this.tile2stageid));
        this.tile3MaterialDatasource = new MatTableDataSource(data.filter(s => s.stageID == this.tile3stageid));
        if (this.tile1noqccount != 0 || this.tile1MaterialCount == 0)
          this.istile1qccount = true;
        else
          this.istile1qccount = false;
        if (this.tile2noqccount != 0 || this.tile2MaterialCount == 0)
          this.istile2qccount = true;
        else
          this.istile2qccount = false;
        if (this.tile3noqccount != 0 || this.tile3MaterialCount == 0)
          this.istile3qccount = true;
        else
          this.istile3qccount = false;
        this.getQcOpenMaterials();
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
  changeClienttile1(value) {
    this.ischecked1 = false;
    this.tile1color = this.stages.find(s => s.id == value).colour;
    this.tile1stagename = this.stages.find(s => s.id == value).name;
    this.tile1MaterialCount = this.materialstatges.filter(s => s.stageID == value).length;
    this.tile1Materialcountdaily = this.materialstatges.filter(s => s.stageID == value && s.createdDate == s.utcDate).length;
    this.satgeorder = this.stages.find(s => s.id == value).order;
    // this.status=this.stages.find(s => s.id == value).order;
    this.tile1MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == value));
    this.tile1QCcount = this.materialstatges.filter(s => s.stageID == this.tile1stageid && s.qCStatus == "All QC passed").length;
    this.tile1noqccount = this.materialstatges.filter(s => s.stageID == value && s.qCStatus == "NA").length;
    if (this.tile1noqccount != 0)
      this.istile1qccount = true;
    else
      this.istile1qccount = false;
    //Destroy  existing donghnutchart
    if (this.doughnutChart)
      this.doughnutChart.destroy();
    this.getQcOpenMaterials();
  }
  changeClienttile2(value) {
    this.ischecked2 = false;
    this.tile2color = this.stages.find(s => s.id == value).colour;
    this.tile2stagename = this.stages.find(s => s.id == value).name;
    this.tile2MaterialCount = this.materialstatges.filter(s => s.stageID == value).length;
    this.tile2Materialcountdaily = this.materialstatges.filter(s => s.stageID == value && s.createdDate == s.utcDate).length;
    this.satgeorder = this.stages.find(s => s.id == value).order;
    this.tile2MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == value));
    this.tile2QCcount = this.materialstatges.filter(s => s.stageID == this.tile2stageid && s.qCStatus == "All QC passed").length;
    this.tile2noqccount = this.materialstatges.filter(s => s.stageID == value && s.qCStatus == "NA").length;
    if (this.tile2noqccount != 0)
      this.istile2qccount = true;
    else
      this.istile2qccount = false;
    //Destroy  existing donghnutchart
    if (this.doughnutChart)
      this.doughnutChart.destroy();
    this.getQcOpenMaterials();

  }
  changeClienttile3(value) {
    this.ischecked3 = false;
    this.tile3color = this.stages.find(s => s.id == value).colour;
    this.tile3stagename = this.stages.find(s => s.id == value).name;
    this.tile3MaterialCount = this.materialstatges.filter(s => s.stageID == value).length;
    this.tile3Materialcountdaily = this.materialstatges.filter(s => s.stageID == value && s.createdDate == s.utcDate).length;
    this.satgeorder = this.stages.find(s => s.id == value).order;
    // this.status=this.stages.find(s=>s.id == value).s
    this.tile3MaterialDatasource = new MatTableDataSource(this.materialstatges.filter(s => s.stageID == value));
    this.tile3QCcount = this.materialstatges.filter(s => s.stageID == value && s.qCStatus == "All QC passed").length;
    this.tile2noqccount = this.materialstatges.filter(s => s.stageID == value && s.qCStatus == "NA").length;
    if (this.tile2noqccount != 0)
      this.istile3qccount = true;
    else
      this.istile3qccount = false;
    //Destroy  existing donghnutchart
    if (this.doughnutChart)
      this.doughnutChart.destroy();
    this.getQcOpenMaterials();
  }

}

