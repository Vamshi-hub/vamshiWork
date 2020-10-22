import { Component, OnInit, ViewChild, ViewChildren, QueryList } from '@angular/core';
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
import { Mrf } from '../classes/mrf-master';
import { MaterialMaster, MaterialMasterLHL } from '../classes/material-master';
import { element } from '@angular/core/src/render3';

@Component({
  selector: 'app-dashboard-consultant',
  templateUrl: './dashboard-consultant.component.html',
  styleUrls: ['./dashboard-consultant.component.css']
})
export class DashboardConsultantComponent extends CommonLoadingComponent {
  
  lineChart = [];
  pieChart = [];
  tiles = [];
  displayedColumns = ['markingNo', 'caseName', 'materialDescription', 'createdOn'];
  delayedDeliveryDisplayedColumns = ['markingNo', 'block', 'level', 'zone', 'type', 'mrfNo', 'expectedDelivery'];
  qcDataSource: MatTableDataSource<QcOpenMaterial> = new MatTableDataSource();
  // Delayed materials
  delayedMaterialDisplayedColumns = ['markingNo', 'orderNo', 'plannedDate', 'actualDate'];
  delayedProductionDS: MatTableDataSource<MaterialMasterLHL> = new MatTableDataSource();
  delayedDeliveryDS: MatTableDataSource<MaterialMasterLHL> = new MatTableDataSource();
  delayedInstallationDS: MatTableDataSource<MaterialMasterLHL> = new MatTableDataSource();
  delayedMaterials: any;

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
  @ViewChildren(MatPaginator) paginators: QueryList<MatPaginator>;

  cardClicked = 'Overall';
  deliveredDataSource = new MatTableDataSource();
  installedDataSource = new MatTableDataSource();
  mrfDataSource = new MatTableDataSource();
  materialDataSource: MatTableDataSource<MaterialMaster> = new MatTableDataSource();
  noData: boolean;
  lstMaterials: MaterialMaster[];
  ToggleView = true;
  level: number = 30;

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
          this.getCharts();
        }
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No selected project', 'OK');
      }
      this.isLoading = false;
    });
  }

  onBlkAClick($element) {
    this.cardClicked = 'BlkA';

    this.tiles = [];
    var colour = "#b0b0b0";
    var marking = 1;
    var totalMaterials = 864;
    var numOfMaterialTypes = 31;
    var highestLevel = 29

    for(var _i = 1; _i <= (numOfMaterialTypes+1)*(highestLevel+1); _i++){
      if (_i%numOfMaterialTypes == 1){
        this.level = this.level - 1;

        if (_i == 1){
          this.tiles.push({text: 'Lvl', color: '#FFFFFF', fontWeight:800})
          
          for (var i=0; i<numOfMaterialTypes; i++)
            this.tiles.push({text: '', color: '#FFFFFF'});
        }
        if (this.level > 1)
          this.tiles.push({text: this.level.toString(), color: '#FFFFFF'})
        else if (this.level == 1)
          this.tiles.push({text: 'Mod', color: '#FFFFFF', fontWeight:800})
        else
          this.tiles.push({text: '', color: '#FFFFFF'})
      }
      
      if (_i > totalMaterials){  
        if (this.level == 1){
          if (marking == 27)
            this.tiles.push({text: 'A' + marking + '/', color: '#FFFFFF'})
          else
            this.tiles.push({text: 'A' + marking, color: '#FFFFFF'})
          
          if (marking == numOfMaterialTypes)
            marking = 1;
          else
            marking = marking+1;
        }
        else if (this.level == 0){
          if (marking == 27)
            this.tiles.push({text: '2A' + marking, color: '#FFFFFF'});
          else  
            this.tiles.push({text: '', color: '#FFFFFF'})

          if (marking == numOfMaterialTypes)
            marking = 1;
          else
            marking = marking+1;
        }
        else if (this.level > 1)
          this.tiles.push({text: '', color: '#000000'})
      }
      else {
        if (_i > 806 || (_i >=776 && _i <=787)) // installed
          colour = "#00dbff";
        else if (_i >=788 && _i <=797)  // delivered
          colour = "#66ff99";
        else if (_i > 797 || (_i >= 745 && _i <=775)) // produced
          colour = "#ffff66";
        else  
          colour = "#b0b0b0";

        var txt = 0;

        /*
        if (_i >= 838 && _i <= 842)
          txt = 3;
        else if ((_i >= 807 && _i <= 809) || (_i >= 788 && _i <= 789))
          txt = 2;
        if ((_i >= 798 && _i <= 802) || (_i >= 790 && _i <= 791))
          txt = 1;
        */
        this.tiles.push({text: txt, color: colour})
      }
    }

    this.level = 30;
    $element.scrollIntoView(false);
  }

  onBlkBClick($element) {
    this.cardClicked = 'BlkB';
    this.tiles = [];

    var colour = "#b0b0b0";
    var marking = 1;
    var totalMaterials = 752;
    var numOfMaterialTypes = 27;
    var highestLevel = 29;

    for(var _i = 1; _i <= (numOfMaterialTypes+1)*highestLevel; _i++){
      if (_i <= 810){
      
        if (_i%numOfMaterialTypes == 1){
          this.level = this.level - 1;

          if (_i == 1){
            this.tiles.push({text: 'Lvl', color: '#FFFFFF', fontWeight:800})
            
            for (var i=0; i<numOfMaterialTypes; i++)
              this.tiles.push({text: '', color: '#FFFFFF'});
          }

          if (this.level > 1)
            this.tiles.push({text: this.level.toString(), color: '#FFFFFF'})
          else if (this.level == 1)
            this.tiles.push({text: 'Mod', color: '#FFFFFF', fontWeight:800})
          else
            this.tiles.push({text: '', color: '#FFFFFF'})
        }
        
        if (_i >= 730 && _i <= 744)
          this.tiles.push({text: '', color: '#000000'});
        else if (_i > totalMaterials){  
          if (this.level == 1){
            if (marking == 16 || marking == 23)
              this.tiles.push({text: 'B' + marking + '/', color: '#FFFFFF'})
            else
              this.tiles.push({text: 'B' + marking, color: '#FFFFFF'})
            
            if (marking == numOfMaterialTypes)
              marking = 1;
            else
              marking = marking+1;
          }
          else if (this.level == 0){
            if (marking == 16 || marking == 23)
              this.tiles.push({text: '2B' + marking, color: '#FFFFFF'});
            else  
              this.tiles.push({text: '', color: '#FFFFFF'})

            if (marking == numOfMaterialTypes)
              marking = 1;
            else
              marking = marking+1;
          }
          else if (this.level > 1)
            this.tiles.push({text: '', color: '#000000'})
        }
        else {
          if (_i >= 703) // installed
            colour = "#00dbff";
          else if (_i >= 676 && _i <= 682)  // delivered
            colour = "#66ff99";
          else if (_i >= 683 && _i <= 694) // produced
            colour = "#ffff66";
          else  
            colour = "#b0b0b0";

          var txt = '';
          /*
          if (_i >= 745 && _i <= 749)
            txt = '3';
          else if ((_i >= 703 && _i <= 705) || (_i >= 676 && _i <= 677))
            txt = '2';
          if ((_i >= 678 && _i <= 679) || (_i >= 683 && _i <= 687))
            txt = '1';
          */

          this.tiles.push({text: txt, color: colour})
        }
      }
    }

    this.level = 30;
    $element.scrollIntoView(false);
  }

  onTileClick(tile, accessRight){
    //if (accessRight >= 2)
      tile.text=(tile.text+1)%4
  }

  getCharts() {
    Chart.defaults.global.defaultFontStyle="'bold'";
    this.materialTrackService.getQcOpenMaterialsByProjectId(this.project.id).subscribe(
      data => {
        this.isLoading = false;

        this.pieChart = new Chart('canDoughnutChart', {
          type: 'pie',
          data: {
            labels: ["Casted", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#ffff66", "#b0b0b0"],
                data: [174, 1427],
              }
            ],
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
            }
          }
        });
        this.pieChart = new Chart('canDoughnutChart2', {
          type: 'pie',
          data: {
            labels: ["Completed", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#66ff99", "#b0b0b0"],
                data: [122, 1479],
                fontWeight: 'bold'
              }
            ]
          },
          datalabels: {
            color: '#000000',
            font: {
              weight: 'bold',
              size: 16,
            }
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart3', {
          type: 'pie',
          data: {
            labels: ["Installed", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#00dbff", "#b0b0b0"],
                data: [105, 1496]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart4', {
          type: 'pie',
          data: {
            labels: ["Casted", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#ffff66", "#b0b0b0"],
                data: [120, 744]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart5', {
          type: 'pie',
          data: {
            labels: ["Completed", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#66ff99", "#b0b0b0"],
                data: [80, 784]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart6', {
          type: 'pie',
          data: {
            labels: ["Installed", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#00dbff", "#b0b0b0"],
                data: [70, 794]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart7', {
          type: 'pie',
          data: {
            labels: ["Casted", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#ffff66", "#b0b0b0"],
                data: [54, 683]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart8', {
          type: 'pie',
          data: {
            labels: ["Completed", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#66ff99", "#b0b0b0"],
                data: [42, 695]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
        this.pieChart = new Chart('canDoughnutChart9', {
          type: 'pie',
          data: {
            labels: ["Installed", "Outstanding",],
            datasets: [
              {
                label: "Population (millions)",
                backgroundColor: ["#00dbff", "#b0b0b0"],
                data: [35, 702]
              }
            ]
          },
          options: {
            cutoutPercentage: 0,
            legend: {
              display: true,
              position: 'bottom'
          }
          }
        });
      },
      error => {
        console.log(error);
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
