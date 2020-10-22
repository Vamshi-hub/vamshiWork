import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { Angular5Csv } from 'angular5-csv/Angular5-csv';
import { FormGroup, FormBuilder, FormControl, Validators } from '../../../../node_modules/@angular/forms';
import { ActivatedRoute, Router } from '../../../../node_modules/@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { Observable } from '../../../../node_modules/rxjs';
import { map, startWith } from '../../../../node_modules/rxjs/operators';
import { ProjectMaster } from '../classes/project-master';
import { ConfigurationService } from '../../configuration/configuration.service';
import { OrganisationMaster } from '../classes/organisation-master';
import { MaterialMasterTemplate } from '../classes/import-material-template';
import { SampleViewComponent } from './sample-view/sample-view.component';
import { OrganisationType } from '../../shared/classes/enums';

@Component({
  selector: 'app-import-material',
  templateUrl: './import-material.component.html',
  styleUrls: ['./import-material.component.css']
})
export class ImportMaterialComponent extends CommonLoadingComponent {
  @ViewChild('fileInput') fileInput;
  importForm: FormGroup;
  project: ProjectMaster;

  blocks: string[];
  materialTypes: string[];
  allOrganisations: OrganisationMaster[];

  filteredBlocks: Observable<string[]>;
  filteredMaterailTypes: Observable<string[]>;
  fileName = "";

  materialTemplate: MaterialMasterTemplate;
  constructor(route: ActivatedRoute, router: Router, private fb: FormBuilder, private cd: ChangeDetectorRef, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService, private configurationService: ConfigurationService) {
    super(route, router);

  }
  ngOnInit() {
    super.ngOnInit();
    this.createImportForm();
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.materialTrackService.getProjectInfo(this.project.id, "").subscribe(
          data => {
            this.blocks=data.blocks;
            this.filteredBlocks = this.importForm.controls["block"].valueChanges
            .pipe(
              startWith(''),
              map(value => this._filterBlock(value))
            );
          });
      }
    }
    );
    this.getMaterialTypes();
    this.getOrganisations();
  }

  createImportForm() {
    this.importForm = this.fb.group({
      block: ['', Validators.required],
      materialType: ['', Validators.required],
      file: [null, Validators.required],
      fileName: new FormControl(),
      organisation: ['', Validators.required]
    });
  }

  private _filterBlock(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.blocks.filter(option => option.toLowerCase().includes(filterValue));
  }

  private _filterMaterialType(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.materialTypes.filter(option => option.toLowerCase().includes(filterValue));
  }

  getMaterialTypes() {
    this.configurationService.getMaterialtypes().subscribe(
      data => {
        this.materialTypes = data;
        this.isLoading = false;
        this.filteredMaterailTypes = this.importForm.controls["materialType"].valueChanges
          .pipe(
            startWith(''),
            map(value => this._filterMaterialType(value))
          );
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  getOrganisations() {
    this.materialTrackService.getListOrganisations().subscribe(
      data => {
        this.allOrganisations = data.filter(o=>o.organisationType==OrganisationType.Vendor);
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      }
    );
  }
  attach() {
    let element: HTMLElement = document.getElementById('importFile') as HTMLElement;
    element.click();
  }

  sampleTemplate()
  {
    this.uiUtilService.openDialog(SampleViewComponent, true);
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
        if (tarr.length > 1)
          lines.push(tarr);
      }
    }
    return lines;
  }

  onFileChange(event) {
    const reader = new FileReader();
    this.importForm.controls['fileName'].setValue('');
    //this.dataSource = null;
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      reader.readAsText([file][0]);
      reader.onload = (e) => {
        var csv = reader.result;
        this.importForm.patchValue({
          file: csv
        });
        this.fileName = [file][0].name;
        this.importForm.controls['fileName'].setValue(this.fileName);
        // need to run CD since file load runs outside of zone
        this.cd.markForCheck();
        
        var lines = this.extractData(csv);
        if (lines.length > 0 && lines[0].length > 1 && lines[0][0] == 'Level' && lines[0][1] == 'Zone' && lines[0][2] == 'Assembly Location' &&lines[0][3]=='Area'&&lines[0][4]=='Dimensions'&&lines[0][5]=='Length'&& lines[0][6]=='DrawingNo'&& lines[0][7] == 'MarkingNo') {
           if (lines.length > 1 && lines[1].length > 2) {
            this.importForm.patchValue({
              file: reader.result
            });
            this.fileName = [file][0].name;
            this.importForm.controls['fileName'].setValue(this.fileName);
            //need to run CD since file load runs outside of zone
            this.cd.markForCheck();
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

    this.materialTemplate = new MaterialMasterTemplate();
    this.materialTemplate = requestBody;


    if (this.fileInput.nativeElement.files.length === 0) {
      this.uiUtilService.openSnackBar("Please select a file", "OK");
      return;
    }

    this.isLoading = true;
    const formData = new FormData();
    formData.append('TemplateFile', this.fileInput.nativeElement.files[0]);
    formData.append('Block', this.materialTemplate.block);
    formData.append('OrganisationID', requestBody["organisation"]);
    formData.append('MaterialType', this.materialTemplate.materialType);


    this.materialTemplate.TemplateFile = formData;
    this.materialTrackService.uploadMaterialFile(formData, this.project.id).subscribe(
      response => {
        if (response["message"] == null) {
          this.uiUtilService.openSnackBar('Upload Successfull', "OK");
          this.router.navigateByUrl('/material-tracking/materials');
        }
        else {
          console.log(response);
          this.uiUtilService.openSnackBar(response["message"], "OK", 10000);
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    )
  }

}


