import { Component, OnInit, ViewChild } from '@angular/core';
import { Role } from '../../shared/classes/role';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-role-master',
  templateUrl: './role-master.component.html',
  styleUrls: ['./role-master.component.css']
})
export class RoleMasterComponent extends CommonLoadingComponent {
  displayedColumns = ['name', 'defaultPage'];
  dataSource: MatTableDataSource<Role> = new MatTableDataSource();
  accessRight = 0;
  Math: any;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
     }

  ngOnInit() {
    super.ngOnInit();
    this.dataSource = new MatTableDataSource();
    this.getRoles();
  }
  roleDetails() {
    this.router.navigateByUrl('/configuration/role-details/0');
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/configuration/role-details/' + row['id']);
  }

  getRoles() {
    this.materialTrackService.getRoles().subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }
        else {
          this.uiUtilService.openSnackBar("No Roles", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        console.log(error);
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
}
