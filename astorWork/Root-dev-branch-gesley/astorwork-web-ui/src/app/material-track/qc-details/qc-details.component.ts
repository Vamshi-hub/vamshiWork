import { Component, OnInit } from '@angular/core';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { ActivatedRoute, ParamMap, Router } from '../../../../node_modules/@angular/router';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { QCDefect } from '../classes/qc-defect';

@Component({
  selector: 'app-qc-details',
  templateUrl: './qc-details.component.html',
  styleUrls: ['./qc-details.component.css']
})
export class QcDetailsComponent extends CommonLoadingComponent {
  defects: QCDefect[];

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.paramMap.subscribe((params: ParamMap) => {
      const id = params.get('id');
      this.getListDefects(+id);
    });
  }

  getListDefects(id: number) {
    this.materialTrackService.getQCDefects(id).subscribe(
      data => {
        console.log(data);
        this.defects = data;
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }
}
