import { Component, OnInit, Inject, ChangeDetectorRef, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA, MatTableDataSource } from '@angular/material';
import { FormBuilder, FormGroup, Validators, FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../../shared/ui-utils.service';
import { JobTrackService } from '../../job-track.service';
import { CommonLoadingComponent } from '../../../shared/common-loading/common-loading.component';
import { importCheckList } from '../../classes/import-master';
import { ProjectMaster } from '../../../material-track/classes/project-master';
import { JobscheduleSampleViewComponent } from '../../jobschedule-sample-view/jobschedule-sample-view.component';

@Component({
  selector: 'app-import-jobschedule',
  templateUrl: './import-jobschedule.component.html',
  styleUrls: ['./import-jobschedule.component.css']
})
export class ImportJobscheduleComponent extends CommonLoadingComponent {
  @ViewChild('fileInput') fileInput;
  displayedColumns = ['block','level','zone','materialType','name', 'message'];
  generateForm: FormGroup;
  importForm: FormGroup;
  dataSource: MatTableDataSource<importCheckList>;
  fileName = "";
  isLoading = false;
  project:ProjectMaster;
  uploadbutton=true;
  constructor(route: ActivatedRoute, private fb: FormBuilder, router: Router, private cd: ChangeDetectorRef, private uiUtilService: UiUtilsService, private jobtrackservice: JobTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.createGenerateForm();
    this.createImportForm();
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
      }
    }
    );
    this.isLoading=false;
  }
  createGenerateForm() {
    this.generateForm = this.fb.group({
      Block: [''],
      Level: [''],
      Unit: [''],
      MaterialType: [''],
      Trade: [''],
      PlannedStartDate: [''],
      PlannedEndDate: ['']


    });
  }
  createImportForm() {
    this.importForm = this.fb.group({
      file: [null, Validators.required],
      // importType: ['', Validators.required],
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
      console.log(data);
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
  sampleTemplate()
  {
    this.uiUtilService.openDialog(JobscheduleSampleViewComponent, true);
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
        console.log(lines);
        if (lines.length > 0 && lines[0][0] == 'Block'&& lines[0][1] == 'Level'&& lines[0][2] == 'Unit'&& lines[0][3] == 'MaterialType'&& lines[0][4] == 'Trade'&& lines[0][5] == 'SubCon'
        && lines[0][6] == 'PlannedStartDate'&& lines[0][7] == 'PlannedEndDate') {
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
      };
    }
  }

  uploadFile(): void {
    let requestBody = this.importForm.value;
    let importType = requestBody["importType"];
    // if (!importType) {
    //   this.uiUtilService.openSnackBar("Please select a import type", "OK");
    //   return;
    // }
    if (this.fileInput.nativeElement.files.length === 0) {
      this.uiUtilService.openSnackBar("Please select a file", "OK");
      return;
    }
    this.isLoading = true;
    const formData = new FormData();
    formData.append('file', this.fileInput.nativeElement.files[0]);
   
      this.jobtrackservice.importJobSchedule(formData, this.project.id).subscribe(
        response => {
          console.log(response);
         this.getResponse(response);      
        },
        error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, 'OK');
        });
   
  }
  getResponse(response: Object): any {
    if (response['length']==0) {
      console.log('Response Lenght: '+response['length']+'');
      this.uiUtilService.openSnackBar('Upload Successfull', "OK");
      this.importForm.controls['fileName'].setValue('');
    }
    else {
      this.uiUtilService.openSnackBar('Upload Unsuccessfull for Some Jobs', "OK");
      this.dataSource = new MatTableDataSource();
      this.dataSource = response as MatTableDataSource<importCheckList>;

    }
    this.isLoading = false;
  }
}
