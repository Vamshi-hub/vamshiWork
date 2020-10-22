import { Component, OnInit, ViewChild, ViewChildren, QueryList, AfterViewInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { ProjectMaster } from '../classes/project-master';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { BIMSyncSession } from '../classes/bim-sync-session';
import { MatTableDataSource, MatPaginator } from '@angular/material';
import { MaterialMaster } from '../classes/material-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-bim-sync-details',
  templateUrl: './bim-sync-details.component.html',
  styleUrls: ['./bim-sync-details.component.css']
})
export class BimSyncDetailsComponent extends CommonLoadingComponent {
  displayedColumns = ['block', 'level', 'zone', 'markingNo'];

  project: ProjectMaster;
  bimSyncDetail: BIMSyncSession;
  dataSourceSynced: MatTableDataSource<MaterialMaster> = new MatTableDataSource();
  dataSourceUnsynced: MatTableDataSource<MaterialMaster> = new MatTableDataSource();

  @ViewChild('paginatorSynced') paginatorSynced: MatPaginator;
  @ViewChild('paginatorUnsynced') paginatorUnsynced: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.route.paramMap.subscribe((params: ParamMap) => {
          const id = params.get('id');
          this.getBImSyncSessionDetail(project.id, +id);
        });
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No projects found', 'OK');
      }
    });
  }

  getBImSyncSessionDetail(projectId: number, sessionId: number) {
    this.materialTrackService.getBIMSyncSessionDetails(projectId, sessionId).subscribe(
      data => {
        this.bimSyncDetail = data;

        this.dataSourceSynced = new MatTableDataSource(this.bimSyncDetail.syncedMaterials);
        this.dataSourceSynced.paginator = this.paginatorSynced;

        this.dataSourceUnsynced = new MatTableDataSource(this.bimSyncDetail.unsyncedMaterials);
        this.dataSourceUnsynced.paginator = this.paginatorUnsynced;

        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  selectMaterialRow(row: any) {
    this.router.navigateByUrl('/material-tracking/materials/' + row['id']);
  }

}
