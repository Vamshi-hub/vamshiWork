import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { FormGroup, Validators, FormControl, FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { QRCode } from '../../material-track/classes/qrCode';
import { Angular5Csv } from 'angular5-csv/Angular5-csv';
import { MatTableDataSource } from '@angular/material';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { ConfigurationService } from '../configuration.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { isEmpty } from 'rxjs/operators';
import { ResultDlgComponent } from '../../material-track/create-mrf/result-dlg/result-dlg.component';
import { isNgTemplate } from '@angular/compiler';

@Component({
  selector: 'app-generate-qr-code',
  templateUrl: './generate-qr-code.component.html',
  styleUrls: ['./generate-qr-code.component.css']
})
export class GenerateQrCodeComponent extends CommonLoadingComponent {
  @ViewChild('fileInput') fileInput;
  generateForm: FormGroup;
  importForm: FormGroup;
  displayedColumns = ['label', 'tag', 'type', 'message'];
  dataSource: MatTableDataSource<QRCode>;
  toggleImpExp = true;
  qrCodes: QRCode[];
  toggle = true;
  fileName = "";
  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private cd: ChangeDetectorRef, private uiUtilService: UiUtilsService, private configurationService: ConfigurationService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.createGenerateForm();
    this.createImportForm();
    this.isLoading = false;
  }

  createGenerateForm() {
    this.generateForm = this.fb.group({
      quantity: new FormControl("", [Validators.required, Validators.max(100), Validators.min(1)]),
      label: ['']
    });
  }

  createImportForm() {
    this.importForm = this.fb.group({
      file: [null, Validators.required],
      tagType: ['', Validators.required],
      fileName: new FormControl(),
    });
  }

  attach() {
    let element: HTMLElement = document.getElementById('importFile') as HTMLElement;
    element.click();
  }
  tabChagne() {
    if (this.toggleImpExp)
      this.toggleImpExp = false;
    else
      this.toggleImpExp = true;
  }

  //#region generate QR code methods
  generate() {
    this.isLoading = true;
    let requestBody = this.generateForm.value;
    let qty = requestBody["quantity"];
    let label = requestBody["label"];
    this.configurationService.getGeneratedQRCodes(qty, label).subscribe(
      data => {
        this.qrCodes = data;
        this.generateForm.controls["quantity"].setValue('');
        this.toggle = false;
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  print() {
    window.print();
    this.qrCodes = null;
    this.toggle = true;
  }
  //#endregion

  //#region import methods

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
        if (tarr.length > 1)
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
        console.log(lines);
        if (lines.length > 0 && lines[0].length > 1 && lines[0][0] == 'Label' && lines[0][1] == 'Tag') {
          if (lines.length > 1 && lines[1].length > 1) {
            this.importForm.patchValue({
              file: reader.result
            }
            );
            this.fileName = [file][0].name;
            this.importForm.controls['fileName'].setValue(this.fileName);
            // need to run CD since file load runs outside of zone
            this.cd.markForCheck();
          }
          else
          {
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
    let tagType = requestBody["tagType"];

    if (!tagType) {
      this.uiUtilService.openSnackBar("Please select a tracker type", "OK");
      return;
    }

    if (this.fileInput.nativeElement.files.length === 0) {
      this.uiUtilService.openSnackBar("Please select a file", "OK");
      return;
    }

    this.isLoading = true;
    const formData = new FormData();
    formData.append('file', this.fileInput.nativeElement.files[0]);
    this.configurationService.uploadFile(formData, tagType).subscribe(
      response => {
        if (response['length']==0) {
          this.uiUtilService.openSnackBar('Upload Successfull', "OK");
        }
        else {
          this.uiUtilService.openSnackBar('Upload Unsuccessfull for some tags', "OK");
          this.dataSource = new MatTableDataSource();
          this.dataSource = response as MatTableDataSource<QRCode>;

        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  //#endregion
}
