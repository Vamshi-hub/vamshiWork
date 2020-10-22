import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { ConfigurationService } from '../configuration.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { StageMaster } from '../../material-track/classes/stage-master';
import { ActivatedRoute, Router } from '@angular/router';
import { DataEvent, DragDropEvent } from '@progress/kendo-angular-sortable';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { Observable, observable } from 'rxjs';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
@Component({
  selector: 'app-stage-master',
  templateUrl: './stage-master.component.html',
  styleUrls: ['./stage-master.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class StageMasterComponent extends CommonLoadingComponent {

  stagemaster: StageMaster[];
  public disabledIndexes = [];
  materilaTypes:string[];
  accessRight = 0;
  theForm: FormGroup;
  lstStages: Observable<StageMaster>;
  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private configService: ConfigurationService) {
    super(route, router);
  }
  ngOnInit() {
    super.ngOnInit();
    this.getMaterialTypes();
   
    
  }

  onSubmit(stage: StageMaster) {
    console.log(stage);
    if ( stage.materialTypes==null||stage.materialTypes.length==0  ) {
        this.uiUtilService.openSnackBar("Please Select Material Type(s)", 'OK');
        return;
      }
    this.isLoading = true;
    if (stage.name == "" || stage.name == null) {
      this.uiUtilService.openSnackBar("Please Enter Satage Name", 'OK');
      return;
    }
    if (stage.colour == "" || stage.colour == null) {
      this.uiUtilService.openSnackBar("Please Select Satage colour", 'OK');
      return;
    }
    this.configService.updateStage(stage.id, stage).subscribe(response => {
      if (response == null) {
        this.uiUtilService.openSnackBar("Stage Updated Successfully", "OK");
        this.getstages();
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(response["message"], "OK");
      }
    },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, "OK");
      });
  }
  SortSave() {
    this.isLoading = true;
    this.configService.updateSorting(this.stagemaster).subscribe(response => {
      if (response == null) {
        this.getstages();
        this.uiUtilService.openSnackBar("Sorting updated Successfully", "OK");
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(response["message"], "OK");
      }
    },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getstages() {
    this.isLoading=true;
    this.configService.getmaterialsstages().subscribe(
      data => {
        this.stagemaster = data;
        // this.stagemaster.forEach(element => {
        //   if (element.isEditable == false)
        //     this.disabledIndexes.push(this.stagemaster.indexOf(element));
        // });
        console.log(this.stagemaster.lastIndexOf);
        
        this.disabledIndexes.push(this.stagemaster.length-1);
        this.isLoading = false;
        console.log(this.stagemaster);
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
  getMaterialTypes() {
    this.isLoading = true;
    this.configService.getMaterialtypes().subscribe(
      data => {
        this.materilaTypes = data;
        this.isLoading = false;
        this.getstages();
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
  public onDataAdd(src: number, e: DataEvent): void {

  }

  public onDataRemove(src: number, e: DataEvent): void {
  }

  public onDragEnd(src: number, e: DragDropEvent): void {
  }

  public onDragOver(src: number, e: DragDropEvent): void {


  }

  public onDragStart(src: number, e: DragDropEvent): void {

  }

}
