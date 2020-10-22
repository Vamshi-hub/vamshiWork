import { Component, OnInit, ViewChild } from '@angular/core';
import { MaterialTrackService } from '../material-track.service';
import { Chart } from 'chart.js';

import { Router, ActivatedRoute, Params } from '@angular/router';
import { MatSort, MatPaginator, MatTableDataSource, MatSelectChange } from '@angular/material';

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
import { MrfMaster } from '../classes/mrf-master';
import { MaterialMaster } from '../classes/material-master';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent extends CommonLoadingComponent {
  lineChart = [];
  doughnutChart = [];
  displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn', 'remarks'];
  delayedDeliveryDisplayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo', 'expectedDelivery'];
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
  @ViewChild(MatSort) DelayedDeliverySort: MatSort;
  @ViewChild(MatSort) QCSort: MatSort;
  @ViewChild(MatSort) DeliveredSort: MatSort;
  @ViewChild(MatSort) InstalledSort: MatSort;
  @ViewChild(MatSort) MRFSort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;
  cardClicked = 'QCOpen';
  deliveredDataSource = new MatTableDataSource();
  installedDataSource = new MatTableDataSource();
  mrfDataSource = new MatTableDataSource();
  materialDataSource: MatTableDataSource<MaterialMaster> = new MatTableDataSource();
  noData: boolean;
  lstMaterials: MaterialMaster[];
  ToggleView = true;
  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
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
      this.router.navigate(['material-tracking', 'qc-defects', row['caseID'], { caseName: row.caseName, title: title }]);
    }
    else if (this.cardClicked == 'InstalledMaterials' || this.cardClicked == 'DeliveredMaterials') {
      this.router.navigate(['material-tracking', 'materials', row['id']]);
    }
    else if (this.cardClicked == 'CompletedMRF') {
      this.router.navigate(['material-tracking', 'materials', { blk: row.block, mrfNo: row.mrfNo }]);
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

  onDeliveredClick() {
    this.cardClicked = 'DeliveredMaterials';
    this.displayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo'];
    if (this.deliveredDataSource.data.length > 0) {
      this.deliveredDataSource.sort = this.DeliveredSort;
    }
  }

  onInstalledClick() {
    this.cardClicked = 'InstalledMaterials';
    this.displayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo'];
    if (this.installedDataSource.data.length > 0) {
      this.installedDataSource.sort = this.InstalledSort;
    }
  }

  onQCOpenClick() {
    this.cardClicked = 'QCOpen';
    this.displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn', 'remarks'];
    if (this.qcDataSource.data.length > 0) {
      this.qcDataSource.sort = this.QCSort;
    }
  }

  onCompletedMRFClick() {
    this.cardClicked = 'CompletedMRF';
    this.displayedColumns = ['mrfNo', 'block', 'level', 'zone', 'type'];
    if (this.mrfDataSource.data.length > 0) {
      this.mrfDataSource.sort = this.MRFSort;
    }
  }

  getProjectStatsById() {
    this.getProjectProgress();
    this.getQcOpenMaterials();
    this.materialTrackService.getProjectStats(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.projectStats = data;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
    this.getMaterials('All');
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
        console.log(error);
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getQcOpenMaterials() {
    this.materialTrackService.getQcOpenMaterialsByProjectId(this.project.id).subscribe(
      data => {
        this.isLoading = false;
        this.qcDataSource = new MatTableDataSource(data["qcOpenMaterialsList"])
        this.deliveredDataSource = new MatTableDataSource(data["deliveredMaterialList"])
        this.installedDataSource = new MatTableDataSource(data["installedMaterialList"])
        this.mrfDataSource = new MatTableDataSource(data["completedMRFList"])

        if (this.qcDataSource.data.length > 0) {
          this.qcDataSource.sort = this.QCSort;
        }

        this.dailyStatus = data["dailyMaterialStatusCount"]
        let deliveredCount = this.dailyStatus.deliveredCount;
        let installedCount = this.dailyStatus.installedCount;
        let startDeliveryCount = this.dailyStatus.startDeliveryCount;
        if (this.dailyStatus.deliveredCount == 0 && this.dailyStatus.installedCount == 0 && this.dailyStatus.startDeliveryCount == 0)
          this.noData = true;
        this.doughnutChart = new Chart('canDoughnutChart', {
          type: 'doughnut',
          data: {
            labels: ["Delivered", "Installed", "Produced",],
            datasets: [
              {
                label: "Poputation (millions)",
                backgroundColor: ["#66ff99", "#3399ff", "#ffff66"],
                data: [deliveredCount, installedCount, startDeliveryCount]
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

  toggleDesign() {
    if (this.ToggleView)
      this.ToggleView = false
    else
      this.ToggleView = true;
  }
}
