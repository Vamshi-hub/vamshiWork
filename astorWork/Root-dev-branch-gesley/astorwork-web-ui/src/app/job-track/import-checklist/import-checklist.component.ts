import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormGroup, Validators, FormControl, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ConfigurationService } from '../../configuration/configuration.service';
import { importCheckList } from '../classes/import-master';
import { MatTableDataSource } from '@angular/material';
import { ProjectMaster } from '../classes/project-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { JobTrackService } from '../job-track.service';
import { TradeMaster } from '../classes/trade-master';
import { StageMaster } from '../../material-track/classes/stage-master';
import { SampleViewComponent } from '../../job-track/import-checklist/sample-view/sample-view.component';
import { ChecklistType } from '../../shared/classes/enums';


@Component({
  selector: 'app-importchecklist',
  templateUrl: './import-checklist.component.html',
  styleUrls: ['./import-checklist.component.css']
})
export class ImportchecklistComponent extends CommonLoadingComponent {
  @ViewChild('fileInput') fileInput;
  displayedColumns = ['name', 'type', 'message'];
  generateForm: FormGroup;
  importForm: FormGroup;
  dataSource: MatTableDataSource<importCheckList>;
  fileName = "";
  project: ProjectMaster;
  isLoading = false;
  importType: string;
  importTemplate: importCheckList;
  trade: TradeMaster[];
  tradeId: number;
  stages: StageMaster[];
  materialStageId: number
  Type: string;
  isShow = false;
  isImportTemplate = true;
  isCheckListTemplate = false;
  isTradeMasterTemplate = false;
  isStages = false;
  isTradeshow = false;
  sampleView: sampleview;
  uploadbutton=true;
  selectfile=false;
  keys : any[];
  checklisttype = ChecklistType;
  constructor(route: ActivatedRoute, private fb: FormBuilder, router: Router, private cd: ChangeDetectorRef, private uiUtilService: UiUtilsService, private jobtrackservice: JobTrackService, private configurationService: ConfigurationService) {
    super(route, router);
    this.keys = Object.keys(this.checklisttype).filter(Number);
  }

  ngOnInit() {
    super.ngOnInit();
    this.createGenerateForm();
    this.createImportForm();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.getTrade();
        this.getMaterialStages();
      }
    }
    );
    this.isLoading = false;

  }
  getMaterialStages() {
    this.configurationService.getMaterialStages().subscribe(
      data => {
        this.stages = data;
      }
    )
  }
  getTrade() {
    this.jobtrackservice.getTrades(this.project.id).subscribe(
      data => {
        this.trade = data;
      }
    )
  }
  createGenerateForm() {
    this.generateForm = this.fb.group({
      Name: ['']
    });
  }
  createImportForm() {
    this.importForm = this.fb.group({
      file: [null, Validators.required],
      importType: ['', Validators.required],
      fileName: new FormControl(),
    });

  }

  attach() {
    let element: HTMLElement = document.getElementById('importFile') as HTMLElement;
    element.click();
  }
  private extractData(data) {

    let csvData = data;
    let allTextLines = csvData.split(/\r\n|\n/);
    let headers = allTextLines[0].split(',');
    let lines = [];

    for (let i = 0; i < allTextLines.length; i++) {
      // split content based on comma
      let data = allTextLines[i].split(',');
      if (data.length == headers.length) {
        let tarr = [];
        for (let j = 0; j < headers.length; j++) {
          if (data[j].replace(/\s/g, "").length > 0)
            tarr.push(data[j]);
        }

        //if (tarr.length > 1)
        lines.push(tarr);
      }
    }
    return lines;

  }

  onFileChange(event) {
    const reader = new FileReader();
    this.importForm.controls['fileName'].setValue('');
    this.importForm.controls['file'].setValue('');
    this.fileName = '';
    this.dataSource = null;
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsText([file][0]);
      reader.onload = (e) => {
        var csv = reader.result;
        var lines = this.extractData(csv);
        let requestBody = this.importForm.value;
        let importType = requestBody["importType"];
        if (importType == 'CHECKLIST') {
          if (lines.length > 0 && lines[0][0] == 'Name' && lines[0][1] == 'TimeFrame' && lines[0][2] == 'Section') {
            if (lines.length > 1) {
              this.importForm.patchValue({
                file: reader.result
              }
              );
              this.fileName = [file][0].name;
              this.importForm.controls['fileName'].setValue(this.fileName);
              // need to run CD since file load runs outside of zone
              this.cd.markForCheck();
              this.selectfile=true;
              if(this.fileName!=null && this.materialStageId!=0)
              this.uploadbutton=false;
              if(this.fileName!=null && this.tradeId!=0)
              this.uploadbutton=false;
            }
            else {
              this.uiUtilService.openSnackBar('Proper data required', "OK");
              event.target.value = "";
            }
          }
          else {
            this.uiUtilService.openSnackBar('Not a proper file. please follow the template', "OK");
            event.target.value = "";
          }
        }
        else if (importType == 'JOB') {
          if (lines.length > 0 && lines[0][0] == 'Name' && lines[0][1] == 'RouteTo'&& lines[0][2] == 'Type'&& lines[0][3]=='RTOInspection') {
            if (lines.length > 1) {
              this.importForm.patchValue({
                file: reader.result
              }
              );
              this.fileName = [file][0].name;
              this.importForm.controls['fileName'].setValue(this.fileName);
              // need to run CD since file load runs outside of zone
              this.cd.markForCheck();
              if(this.fileName!=null)
              this.uploadbutton=false;
            }
            else {
              this.uiUtilService.openSnackBar('Proper data required', "OK");
              event.target.value = "";
            }
          }
          else {
            this.uiUtilService.openSnackBar('Not a proper file. please follow the template', "OK");
            event.target.value = "";
          }
        }
        else {
          if (lines.length > 0 && lines[0][0] == 'Name'&& lines[0][1] == 'RouteTo') {
            if (lines.length > 1) {
              this.importForm.patchValue({
                file: reader.result
              }
              );
              this.fileName = [file][0].name;
              this.importForm.controls['fileName'].setValue(this.fileName);
              // need to run CD since file load runs outside of zone
              this.cd.markForCheck();
              if(this.fileName!=null)
              this.uploadbutton=false;
            }
            else {
              this.uiUtilService.openSnackBar('Proper data required', "OK");
              event.target.value = "";
            }
          }
          else {
            this.uiUtilService.openSnackBar('Not a proper file. please follow the template', "OK");
            event.target.value = "";
          }
        }
      };
    }
  }

  uploadFile(): void {
    let requestBody = this.importForm.value;
    let importType = requestBody["importType"];
    if (!importType) {
      this.uiUtilService.openSnackBar("Please select a import type", "OK");
      return;
    }
    if (this.fileInput.nativeElement.files.length === 0) {
      this.uiUtilService.openSnackBar("Please select a file", "OK");
      return;
    }
    this.isLoading = true;
    const formData = new FormData();
    formData.append('file', this.fileInput.nativeElement.files[0]);

    if (importType == 'CHECKLIST') {
      this.jobtrackservice.importChecklist(formData, this.project.id, this.tradeId, this.Type,this.materialStageId).subscribe(
        response => {
          this.getResponse(response, importType);
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, 'OK');
        });
    }
    else if (importType == 'JOB') {
      this.jobtrackservice.importJob(formData, this.project.id).subscribe(
        response => {
          this.getResponse(response, importType);
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, 'OK');
        });
    }
    else if (importType == 'MATERIALTYPE') {
      this.jobtrackservice.importMaterialTypes(formData, this.project.id).subscribe(
        response => {
          this.getResponse(response, importType);
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, 'OK');
        });
    }

  }
  sampleTemplate()
  {
        this.sampleView = new sampleview();
        this.sampleView.ischecklisttemplate=this.isCheckListTemplate;
        this.sampleView.istradeMastertemplate=this.isTradeMasterTemplate;
        this.sampleView.isstages=this.isStages;
        this.sampleView.ismaterialtype=this.isImportTemplate;
        this.sampleView.tradeid=this.tradeId;

    this.uiUtilService.openDialog(SampleViewComponent,this.sampleView, true);
  }

  OnCheckListChange(value) {
    console.log(value);
    if (value == 'CHECKLIST') {
      this.isShow = true;
      this.isImportTemplate = false;
      this.isCheckListTemplate = true;
      this.isTradeMasterTemplate = false;
      this.isTradeshow=false;
      this.importForm.controls['fileName'].setValue('');
      this.Type=null;
      this.uploadbutton=true;
     
    }
    else if (value == 'JOB') {
      this.isTradeMasterTemplate = true;
      this.isImportTemplate = false;
      this.isCheckListTemplate = false;
      this.isShow = false;
      this.isTradeshow=false;
      this.isStages = false;
      this.importForm.controls['fileName'].setValue('');
      this.uploadbutton=true;
    }
    else {
      this.isShow = false;
      this.isImportTemplate = true;
      this.isCheckListTemplate = false;
      this.isTradeMasterTemplate = false;
      this.isTradeshow=false;
      this.isStages = false;
      this.importForm.controls['fileName'].setValue('');
      this.uploadbutton=true;
    }

  }
  OnTradeChange(value) {
    if( this.selectfile==true && this.Type!=null && this.tradeId !=null ){

      this.uploadbutton=false;
    }
        else{
          this.uploadbutton=true;
        }
        console.log(this.fileName)
  }
  OnStageChange(value){
    if(  this.selectfile==true && this.Type!=null && this.materialStageId !=null )
    this.uploadbutton=false;
    else{
      this.uploadbutton=true;

    }
    console.log(this.fileName)
  }
  OnListChange(value) {
    if (value == 2||value==3) {
      this.isStages = true;
      this.isTradeshow = false;
      this.tradeId=0;
      this.importForm.controls['fileName'].setValue('');
      this.uploadbutton=true;
      this.selectfile=false;
    }
    else
    {
      this.isStages = false;
      this.isTradeshow = true;
      this.materialStageId=0;
      this.importForm.controls['fileName'].setValue('');
      this.uploadbutton=true;
      this.selectfile=false;
    }
  }
  getResponse(response: Object, importType: string): any {
    if (response['length'] == 0) {
      this.uiUtilService.openSnackBar('Upload Successfull', "OK");
      this.isShow = false;
      this.isShow = false;
      this.isTradeshow=false;
      this.isStages = false;
      this.importForm.controls['fileName'].setValue('');
      this.importType=null;
      this.Type=null;
      this.materialStageId=0;
      this.tradeId=0;
      this.uploadbutton=true;
    }
    else {
      this.uiUtilService.openSnackBar('Upload Unsuccessfull for ' + importType, "OK");
      this.dataSource = new MatTableDataSource();
      this.dataSource = response as MatTableDataSource<importCheckList>;

    }
    this.isLoading = false;
  }

}


export class sampleview {
  ischecklisttemplate:boolean;
  istradeMastertemplate:boolean;
  isstages:boolean;
  ismaterialtype:boolean;   
  tradeid:number;
 }
 