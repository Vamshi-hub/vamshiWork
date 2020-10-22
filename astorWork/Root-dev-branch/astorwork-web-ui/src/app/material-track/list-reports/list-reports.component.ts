import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { PowerBIReport } from '../classes/power-bi-report';

@Component({
  selector: 'app-list-reports',
  templateUrl: './list-reports.component.html',
  styleUrls: ['./list-reports.component.css']
})
export class ListReportsComponent extends CommonLoadingComponent {

  power_bi_token: string;
  power_bi_reports: PowerBIReport[];

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.getPowerBIReport();
  }

  getPowerBIReport() {
    this.materialTrackService.getPowerBIToken().subscribe(token => {
      if (token) {
        this.power_bi_token = token;
        this.materialTrackService.getPowerBIReports(this.power_bi_token).subscribe(data => {
          this.power_bi_reports = data;
          this.isLoading = false;
        }, error => {
          this.isLoading = false;
          this.uiUtilService.openSnackBar(error, 'OK');
        });
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('Fail to validate with Power BI', 'OK');
      }

    }, error => {
      this.isLoading = false;
      this.uiUtilService.openSnackBar(error, 'OK');
    });
  }

}
