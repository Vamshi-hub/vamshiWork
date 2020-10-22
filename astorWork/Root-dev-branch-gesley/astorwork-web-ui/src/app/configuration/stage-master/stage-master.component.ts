import { Component, OnInit, ViewEncapsulation, ViewChild } from '@angular/core';
import { ConfigurationService } from '../configuration.service';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { StageMaster } from '../../material-track/classes/stage-master';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup } from '@angular/forms';
import { Observable } from 'rxjs';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { CdkDragDrop, moveItemInArray, CdkDrag, transferArrayItem } from '@angular/cdk/drag-drop';
import { MatOption, MatSelect } from "@angular/material";
@Component({
  selector: 'app-stage-master',
  templateUrl: './stage-master.component.html',
  styleUrls: ['./stage-master.component.css'],
  encapsulation: ViewEncapsulation.None,
})
export class StageMasterComponent extends CommonLoadingComponent {
  newStages: StageMaster[];
  stages: StageMaster[];
  public disabledIndexes = [];
  materialTypes: string[];
  accessRight = 0;
  theForm: FormGroup;
  lstStages: Observable<StageMaster>;
  allSelected = false;
  @ViewChild('mySel') mySel: MatSelect;
  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private configService: ConfigurationService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.getMaterialTypes();
    this.newStages = new Array<StageMaster>();
  }

  onDrop(event: CdkDragDrop<StageMaster[]>) {
    if (event.currentIndex > 0 && event.currentIndex < this.stages.length - 1) {
      if (event.previousContainer === event.container) {
        moveItemInArray(this.stages, event.previousIndex, event.currentIndex);
      } else {
        transferArrayItem(this.newStages,
          this.stages,
          event.previousIndex,
          event.currentIndex);
      }
    }
  }

  onSubmit(stage: StageMaster) {
    if (stage.materialTypes == null || stage.materialTypes.length == 0) {
      this.uiUtilService.openSnackBar("Please Select Material Type(s)", 'OK');
      return;
    }
    this.isLoading = true;
    if (stage.name == "" || stage.name == null) {
      this.uiUtilService.openSnackBar("Please Enter Stage Name", 'OK');
      return;
    }
    if (stage.colour == "" || stage.colour == null) {
      this.uiUtilService.openSnackBar("Please Select Stage colour", 'OK');
      return;
    }
    this.configService.updateStage(stage.id, stage).subscribe(response => {
      if (response == null) {
        this.uiUtilService.openSnackBar("Stage Updated Successfully", "OK");
        this.getStages();
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

  onCancel() {
    window.location.reload();
  }

  onSave() {
    this.isLoading = true;
    this.stages.forEach((item, index) => {
      item.order = index + 1;
      if(Array.isArray(item.materialTypeArray) && item.materialTypeArray.length)
      item.materialTypes = item.materialTypeArray.join(',');
      else
      item.materialTypes=null;
    });
    
    this.newStages = new Array<StageMaster>();
    this.configService.updateSorting(this.stages).subscribe(data => {
      this.isLoading = false;
      if (data) {
        data.forEach(item => {
          item.materialTypeArray = item.materialTypes.split(',');
        });
        this.stages = data;
        this.uiUtilService.openSnackBar("Stages updated Successfully", "OK");
      }
      else {
        this.uiUtilService.openSnackBar("Unable to update stages", "OK");
      }
    },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getStages() {
    this.isLoading = true;
    this.configService.getMaterialStages().subscribe(
      data => {
        data.forEach(item => {
          //item.materialTypeArray.splice(0,0,'ALL');
         if(item.materialTypes !=null)
          item.materialTypeArray = item.materialTypes.split(',');
        });
        this.stages = data;

        this.isLoading = false;
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
        this.materialTypes = data;
        this.isLoading = false;
        this.getStages();
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  createNewStage() {
    let newStage = new StageMaster(true);
    this.newStages.push(newStage);
  }

  /** Predicate function that only allows valid stage into a list. */
  createNewPredicate(item: CdkDrag<StageMaster>) {
    if (item.data.name && item.data.colour && item.data.materialTypeArray)
      return true;
    else
      return false;
  }

  /** Predicate function that doesn't allow items to be dropped into a list. */
  noReturnPredicate() {
    return false;
  }
  toggleAllSelection(item:StageMaster) {
    this.allSelected = !this.allSelected; 
    if (this.allSelected) {
     item.materialTypeArray=this.materialTypes;
    } else {
    item.materialTypeArray=null;
    }
  }

}
