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

export interface Cell {
  color: string;
  textcolor:string;
  cols: number;
  rows: number;
  text: string;
  fontsize: number;
}

@Component({
  selector: 'app-dashboard-alec',
  templateUrl: './dashboard-alec.component.html',
  styleUrls: ['./dashboard-alec.component.css']
})

export class DashboardAlecComponent extends CommonLoadingComponent {
  ToggleView = true;
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
  level: number = 30;

  shadedHeaderColour = 'lightpurple';
  unshadedHeaderColour = 'lightgrey';
  textColour = 'black';
  drawingNo;

  cells: Cell[] = [
    //{text: '', cols: 14, rows: 2, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    
    {text: 'Total No. Items', cols: 2, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Total Sqm', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    
    {text: '1', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '2', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '3', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '4', cols: 2, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '5', cols: 2, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '6', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '7', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '8', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '9', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Total', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: '', cols: 1, rows: 1, color: 'white', textcolor: this.textColour, fontsize: 10},
    
    {text: '0', cols: 2, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: '0', cols: 2, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 2, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour,fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: '0', cols: 1, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: '', cols: 1, rows: 1, color: 'white', textcolor: this.textColour, fontsize: 10},

    {text: 'Job Name', cols: 3, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Item No', cols: 2 , rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Qty', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    //{text: 'Q/C + Loaded', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour},
    {text: 'Description', cols: 2, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'End 1 (mm)', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'End 2 (mm)', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'End 3 (mm)', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'End 4 (mm)', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Length', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Area', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Material', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Class / Spec', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Notes', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'MNFD', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'QC', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'TRAN', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'DELSITE', cols: 2, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'DELAREA', cols: 2, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'INST', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'APVD INT', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'APVD EXT', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'SCRP', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'REF', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
    {text: 'Current Status', cols: 1, rows: 1, color: this.shadedHeaderColour, textcolor: this.textColour, fontsize: 10},
  ];


  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      var totalQty = 0;
      var totalArea = 0;
      var MNFD = 0;
      var QC = 0;
      var TRAN = 0;
      var DELSITE = 0;
      var DELAREA = 0;
      var INST = 0;
      var APVDINT = 0;
      var APVDEXT = 0;
      var SCRP = 0;

      this.materialTrackService.getMaterials(project.id, "All").subscribe(
        data => {
          this.drawingNo = data[0].drawingNo;

          var materialCount = data.length;
          var blankColsCount = 13;
          var totalColsCount = 24;
          var length = 0;
          var stageNames = ['MNFD', 'QC', 'TRAN', 'DELSITE', 'DELAREA', 'INST', 'APVD INT', 'APVDEXT', 'SCRP']
          var currentStageIndex = -1;

          for(var materialRow = 0; materialRow < materialCount; materialRow++){
            var qty = 1;
            for(var colIndex = 0; colIndex < blankColsCount; colIndex++){
              var colSpan = 1;
              
              var txt;
              var area = data[materialRow].area;
              
              if (colIndex == 0){
                colSpan = 3;
                txt = data[materialRow].drawingNo;
              }
              else if (colIndex == 1){
                colSpan = 2;
                txt = "L" + data[materialRow].level + "/" + data[materialRow].markingNo
              }
              else if (colIndex == 2)
                txt = 1;
              else if (colIndex == 3){
                colSpan = 2;
                txt = data[materialRow].materialType;
              }
              else if (colIndex == 8){  
                txt = data[materialRow].length;
                length = data[materialRow].length;
              }
              else if (colIndex == 9)
                txt = data[materialRow].area;
              else 
                txt = '';

              this.cells.push({text: txt, cols: colSpan, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10})
            }
    
            totalQty = totalQty + qty;
            totalArea = totalArea + area;
            
            for(var k = blankColsCount; k < totalColsCount-2; k++){
              var colSpan = 1;
              var stageOrder = data[materialRow].stageOrder;

              if ((stageOrder == 1 && data[materialRow].qcStatus == "12") || stageOrder > 1 )
                stageOrder++;
              if (stageOrder == 5){
                if (data[materialRow].qcStatus == "9")
                  stageOrder++;
                if (data[materialRow].qcStatus == "12")
                  stageOrder++;
              }

              txt = 'N';
              var colour = 'red';
    
              if (k == 16 || k == 17)
                colSpan = 2;
              if (k < blankColsCount + stageOrder){
                txt = 'Y';
                colour = 'green';
                currentStageIndex = currentStageIndex + 1;
              }

              if (txt == 'Y'){
                if (k == 14)
                  MNFD = MNFD + length;
                else if (k == 15)
                  QC = QC + length;
                else if (k == 16)
                  TRAN = TRAN + length;
                else if (k == 17)
                  DELSITE = DELSITE + length;
                else if (k == 18)
                  DELAREA = DELAREA + length;
                else if (k == 19)
                  INST = INST + length;
                else if (k == 20)
                  APVDINT = APVDINT + length;
                else if (k == 21)
                  APVDEXT = APVDEXT + length;
                else if (k == 22)
                  SCRP = SCRP + length;
              }

              this.cells.push({text: txt, cols: colSpan, rows: 1, color: colour, textcolor: 'white', fontsize: 10})
            }     
            
            for(var k = totalColsCount-2; k < totalColsCount; k++){
              var colSpan = 1;
              txt = 'None'
              if (k == totalColsCount-2)
                txt = currentStageIndex + 1;
              else if (k == totalColsCount-1)
                txt = stageNames[currentStageIndex];
              
              this.cells.push({text: txt, cols: colSpan, rows: 1, color: this.unshadedHeaderColour, textcolor: this.textColour, fontsize: 10})
            }   

            length = 0;
            currentStageIndex = -1;
          }
    
          this.cells[13].text = totalQty.toString();
          this.cells[14].text = totalArea.toFixed(2).toString();
          this.cells[15].text = MNFD.toString();
          this.cells[16].text = QC.toString();
          this.cells[17].text = TRAN.toString();
          this.cells[18].text = DELSITE.toString();
          this.cells[19].text = DELAREA.toString();
          this.cells[20].text = INST.toString();
          this.cells[21].text = APVDINT.toString();
          this.cells[22].text = APVDEXT.toString();
          this.cells[23].text = SCRP.toString();
          this.cells[24].text = (totalQty + totalArea + MNFD + QC + TRAN + DELSITE + DELAREA + INST + APVDINT + APVDEXT + SCRP).toString();
          this.isLoading = false;     
        },
        error => {
          this.uiUtilService.closeAllDialog();
          this.uiUtilService.openSnackBar(error, 'OK');
        });  
      
    }); 
  }

  toggleDesign() {
    if (this.ToggleView)
      this.ToggleView = false
    else
      this.ToggleView = true;
  }
}