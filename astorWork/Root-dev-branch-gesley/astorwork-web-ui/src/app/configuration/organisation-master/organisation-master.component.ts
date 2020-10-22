import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator, MatSelectChange } from '@angular/material';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { Router, ActivatedRoute } from '@angular/router';
import { OrganisationMaster } from '../../material-track/classes/organisation-master';
import { ConfigurationService } from '../configuration.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';


@Component({
  selector: 'app-organisation-master',
  templateUrl: './organisation-master.component.html',
  styleUrls: ['./organisation-master.component.css']
})
export class OrganisationMasterComponent extends CommonLoadingComponent {
  displayedColumns = ['name', 'organisationTypeName', 'cycleDays'];
  dataSource: MatTableDataSource<OrganisationMaster> = new MatTableDataSource();
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
    this.getOrganisations();
  }
  profile() {
    this.router.navigateByUrl('/material-tracking/organisation-details/0');
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/material-tracking/organisation-details/' + row['id']);
  }

  getOrganisations() {
    this.configurationService.getOrganisations().subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }
        else {
          this.uiUtilService.openSnackBar("No Organisations", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }
}
