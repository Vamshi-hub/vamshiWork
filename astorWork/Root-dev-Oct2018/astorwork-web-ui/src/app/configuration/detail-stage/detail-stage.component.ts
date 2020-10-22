import { Component, OnInit } from '@angular/core';
import { StageMaster } from '../../material-track/classes/stage-master';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfigurationService } from '../configuration.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-detail-stage',
  templateUrl: './detail-stage.component.html',
  styleUrls: ['./detail-stage.component.css']
})
export class DetailStageComponent extends CommonLoadingComponent 
{
 theForm: FormGroup;
 stage: StageMasterSave;
 PreviousStag:number;
 Stages:StageMaster[];
 stageMasterSave:StageMasterSave;
 materialTypes:string[];
 constructor(route: ActivatedRoute, router: Router,private fb: FormBuilder,private uiUtilService: UiUtilsService, private configService: ConfigurationService) {
  super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.stage= new StageMasterSave();
    this.isLoading = false;
    this.getstages();
    this.getMaterialTypes();
    this.createForm();
  }
  createForm() {
    this.theForm = this.fb.group({
      name: ['', [Validators.required]],
      nextStageId: ['', Validators.required],
      materialTypes: ['', Validators.required]

    });
  }
  OnCancel()
  {
    this.router.navigateByUrl('/configuration/stage-master');
  }
  OnSave()
{
  let requestBody = this.theForm.value;
  this.stage.name = requestBody['name'];
    if(this.stage.colour==null)
    {
    this.uiUtilService.openSnackBar("Please Select Satage colour", 'OK');
    return;
    }
    this.isLoading = true;
    console.log(this.stage);
    this.configService.createStage(this.stage).subscribe( response => {
      if (response['message'] == null) {
        this.uiUtilService.openSnackBar("Stage created successfully","OK");
        this.router.navigateByUrl('/configuration/stage-master');
      }
      else
      {
        this.uiUtilService.openSnackBar(response['message'],"OK");
      }
      this.isLoading = false;
    },
    error =>{
      this.isLoading = false;
      this.uiUtilService.openSnackBar(error,"OK");
    }
  );
  }
  getstages() {
    this.configService.getmaterialsstages().subscribe(
      data => {
        this.Stages = data;
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
  getMaterialTypes() {
    this.configService.getMaterialtypes().subscribe(
      data => {
        this.materialTypes = data;
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
}
export class StageMasterSave {
  name: string;
  colour: string;
  isqc: boolean;
  nextStageId:number;
  materialTypes:string[];
}