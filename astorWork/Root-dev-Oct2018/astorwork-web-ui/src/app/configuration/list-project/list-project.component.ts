import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator } from '@angular/material';
import { ProjectMaster } from '../../material-track/classes/project-master';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { Country } from '../classes/country';

@Component({
  selector: 'app-list-project',
  templateUrl: './list-project.component.html',
  styleUrls: ['./list-project.component.css']
})
export class ListProjectComponent extends CommonLoadingComponent {
  displayedColumns = ['name', 'projectManagerName', 'description', 'country', 'startDate', 'endDate'];
  dataSource: MatTableDataSource<ProjectMaster> = new MatTableDataSource();

  Math: any;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) { 
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
 
    this.dataSource = new MatTableDataSource();
    this.getProjects();
  }
  createProject() {
    this.router.navigateByUrl('/configuration/project-details/0');
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/configuration/project-details/' + row['id']);
  }

  getProjects() {
    this.materialTrackService.getListProjects().subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
        }
        else {
          this.uiUtilService.openSnackBar("No Users", 'OK');
        }
        this.isLoading=false;
      },
      error => {
        this.isLoading=false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
      
  }
}
