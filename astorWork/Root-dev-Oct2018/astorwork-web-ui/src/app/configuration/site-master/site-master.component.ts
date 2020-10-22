import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { ConfigurationService } from '../configuration.service';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { SiteMaster } from '../classes/site-master';

@Component({
  selector: 'app-site-master',
  templateUrl: './site-master.component.html',
  styleUrls: ['./site-master.component.css']
})
export class SiteMasterComponent extends CommonLoadingComponent {
  displayedColumns = ['name','description', 'country', 'vendorName'];
  dataSource: MatTableDataSource<SiteMaster> = new MatTableDataSource();
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
    this.getSites();
  }

  selectRow(row: SiteMaster){
    this.router.navigate(['configuration', 'site-details', row.id]);
  }

  navigateCreate(){
    
  }

  getSites() {
    this.configurationService.getSites().subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }
        else {
          this.uiUtilService.openSnackBar("No Sites", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

}
