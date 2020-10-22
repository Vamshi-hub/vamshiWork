import { Component, OnInit, ViewChild } from '@angular/core';
import { LocationMaster } from '../../material-track/classes/location-master';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ConfigurationService } from '../configuration.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-location-master',
  templateUrl: './location-master.component.html',
  styleUrls: ['./location-master.component.css']
})
export class LocationMasterComponent extends CommonLoadingComponent {
  displayedColumns = ['name', 'siteName', 'description', 'type'];
  dataSource: MatTableDataSource<LocationMaster> = new MatTableDataSource();
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
    this.getLocations();
  }

  selectRow(row: LocationMaster){
    this.router.navigate(['configuration', 'location-details', row.id]);
  }

  navigateCreate(){
    
  }

  getLocations() {
    this.configurationService.getLocations().subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }
        else {
          this.uiUtilService.openSnackBar("No Locations", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

}
