import { Component, OnInit, ViewChild } from '@angular/core';
import { MatPaginator, MatSort, MatTableDataSource } from '@angular/material';
import { BIMSyncSession } from '../classes/bim-sync-session';
import { ProjectMaster } from '../classes/project-master';
import { ActivatedRoute, Router } from '@angular/router';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { VideoDlgComponent } from '../../shared/video-dlg/video-dlg.component';
import * as moment from 'moment';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';

@Component({
  selector: 'app-list-bim-sync',
  templateUrl: './list-bim-sync.component.html',
  styleUrls: ['./list-bim-sync.component.css']
})

export class ListBimSyncComponent extends CommonLoadingComponent {
  displayedColumns = ['bimModelId', 'personName', 'countSyncedMaterials', 'countUnsyncedMaterials', 'videoURL', 'syncTime'];
  dataSource: MatTableDataSource<BIMSyncSession> = new MatTableDataSource();
  project: ProjectMaster;
  filterBIM = '';
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      this.dataSource = new MatTableDataSource();

      const project = await (data.project);
      if (project) {
        this.project = project;
        this.getBIMSyncSessionByProject();
      }
      else {
        this.isLoading = false;
        this.uiUtilService.openSnackBar('No projects found', 'OK');
      }
    });
  }

  getBIMSyncSessionByProject() {
    this.materialTrackService.getListBIMSyncSessions(this.project.id).subscribe(data => {
      if (data && data.length > 0) {
        this.dataSource = new MatTableDataSource(data);
        this.dataSource.paginator = this.paginator;
        this.dataSource.sort = this.sort;
        this.dataSource.filterPredicate = (data: BIMSyncSession, filter: string) => {
          const filterArray = filter.split(' ');
          let countMatch = 0;
          for (let entry of filterArray) {
            if (data.bimModelId.toLowerCase().indexOf(entry) >= 0 ||
              data.personName.toLowerCase().indexOf(entry) >= 0)
              countMatch++;
          }
          if (countMatch == filterArray.length)
            return true;
          else
            return false;
        };
      }
      else {
        this.uiUtilService.openSnackBar("This project doesn't have any BIM sync sessions", 'OK');
      }
      this.isLoading = false;

    }, error => {
      this.isLoading = false;
      this.uiUtilService.openSnackBar(error, 'OK');
    });
  }

  selectRow(row: any) {
    this.router.navigateByUrl('/material-tracking/bim-syncs/' + row['id']);
  }

  playVideo(bimSync: BIMSyncSession) {
    const syncTimeStr = moment(bimSync.syncTime).format('MM/Do/YYYY, HH:mm:ss');
    if (document.body.clientWidth >= 600) {
      const videoData = {
        videoTitle: `${bimSync.bimModelId} synced at ${syncTimeStr}`,
        videoWidth: 640,
        videoHeight: 480,
        videoUrl: bimSync.videoURL
      };
      this.uiUtilService.openDialog(VideoDlgComponent, videoData, false);
    }
    else if (window.confirm("This will open a video which may cost in a non-WiFi environment, are you sure?")) {
      const syncTimeStr = moment(bimSync.syncTime).format('MM/Do/YYYY, HH:mm:ss');
      const videoData = {
        videoTitle: `${bimSync.bimModelId} synced at ${syncTimeStr}`,
        videoWidth: 320,
        videoHeight: 240,
        videoUrl: bimSync.videoURL
      }
      this.uiUtilService.openDialog(VideoDlgComponent, videoData, false);
    }
  }

  onFilterKeyIn(event: KeyboardEvent) {
    var filterValue = (<HTMLInputElement>event.target).value;
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

}
