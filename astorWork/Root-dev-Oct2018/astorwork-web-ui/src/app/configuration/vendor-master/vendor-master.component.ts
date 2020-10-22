import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator, MatSelectChange } from '@angular/material';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { Router, ActivatedRoute } from '@angular/router';
import { VendorMaster } from '../../material-track/classes/vendor-master';
import { ConfigurationService } from '../configuration.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';


@Component({
  selector: 'app-vendor-master',
  templateUrl: './vendor-master.component.html',
  styleUrls: ['./vendor-master.component.css']
})
export class VendorMasterComponent extends CommonLoadingComponent {
  displayedColumns = ['name', 'cycleDays'];
  dataSource: MatTableDataSource<VendorMaster> = new MatTableDataSource();
  accessRight = 0;

  Math: any;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private configurationService: ConfigurationService) {
    super(route, router);
   }

  ngOnInit() {
    super.ngOnInit();
    this.dataSource = new MatTableDataSource();
    this.getVendors();
  }
  profile() {
    this.router.navigateByUrl('/configuration/vendor-details/0');
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/configuration/vendor-details/' + row['id']);
  }

  getVendors() {
    this.configurationService.getVendors().subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }
        else {
          this.uiUtilService.openSnackBar("No Vendors", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
}
