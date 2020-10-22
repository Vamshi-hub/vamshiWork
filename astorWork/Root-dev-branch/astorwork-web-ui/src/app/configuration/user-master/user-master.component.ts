import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator, MatSelectChange } from '@angular/material';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { Router, ActivatedRoute } from '@angular/router';
import { Users } from '../../material-track/classes/users';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';


@Component({
  selector: 'app-user-master',
  templateUrl: './user-master.component.html',
  styleUrls: ['./user-master.component.css']
})
export class UserMasterComponent extends CommonLoadingComponent {
  displayedColumns = ['userName', 'email', 'role', 'projectName', 'site', 'organisationName', 'isActive'];
  dataSource: MatTableDataSource<Users> = new MatTableDataSource();
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
    this.getUsers();
  }
  profile() {
    this.router.navigateByUrl('/configuration/user-details/0');
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/configuration/user-details/' + row['userID']);
  }

  getUsers() {
    this.materialTrackService.getUsers(null).subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {

          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        
          
        }
        else {
          this.uiUtilService.openSnackBar("No Users", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
}
