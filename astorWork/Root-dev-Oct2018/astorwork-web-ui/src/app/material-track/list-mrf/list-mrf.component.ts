import { Component, OnInit, ViewChild } from '@angular/core';
import { MatTableDataSource, MatSort, MatPaginator, MatSelectChange } from '@angular/material';
import { MrfMaster } from '../classes/mrf-master';
import { UiUtilsService } from '../../shared/ui-utils.service';
import { MaterialTrackService } from '../material-track.service';
import { ProjectMaster } from '../classes/project-master';
import { SpinnerDlgComponent } from '../../shared/spinner-dlg/spinner-dlg.component';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { filter } from 'rxjs/operators';

@Component({
  selector: 'app-list-mrf',
  templateUrl: './list-mrf.component.html',
  styleUrls: ['./list-mrf.component.css']
})
export class ListMrfComponent extends CommonLoadingComponent {
  displayedColumns = ['mrfNo', 'block', 'level', 'zone', 'materialTypes', 'vendorName', 'orderDate', 'plannedCastingDate', 'progress'];
  dataSource: MatTableDataSource<MrfMaster> = new MatTableDataSource();
  project: ProjectMaster;
  blocks = [];
  selectedBlock = 'all';
  filter = '';

  Math: any;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.Math = Math;
    this.dataSource = new MatTableDataSource();
    
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      await this.route.paramMap.subscribe((params: Params) => {
        this.filter = params.get('filter') == null ? '' : params.get('filter');
      });

      const project = await (data.project);
      if (project) {
        this.project = project;
        this.blocks = this.project.blocks;
        this.getMRFsByBlk();
        
      }
      else {
        this.uiUtilService.openSnackBar('No projects found', 'OK');
        this.isLoading = false;
      }
      
    });
  }

  onBlockChanged(event: MatSelectChange) {
    if (event.value == 'all') {
      this.getMRFsByBlk();
    }
    else {
      this.getMRFsByBlk(event.value);
    }
  }

  selectRow(row: any) {
    this.router.navigate(['../materials', { blk: row.block, mrfNo: row.mrfNo }], { relativeTo: this.route })
  }

  onFilterKeyIn(event: KeyboardEvent) {
    var filterValue = (<HTMLInputElement>event.target).value;
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.dataSource.filter = filterValue;
  }

  getMRFsByBlk(blk?: string) {
    this.materialTrackService.getListMRFs(this.project.id, blk).subscribe(
      data => {
        this.dataSource = new MatTableDataSource(data);
        if (this.dataSource.data.length > 0) {
          this.dataSource.paginator = this.paginator;
          this.dataSource.sort = this.sort;
          this.dataSource.filterPredicate = (data: MrfMaster, filter: string) => {
            const filterArray = filter.split(' ');
            let countMatch = 0;
            for (let entry of filterArray) {
              if (data.level.toLowerCase().indexOf(entry) >= 0 ||
                data.mrfNo.toLowerCase().indexOf(entry) >= 0 ||
                data.zone.toLowerCase().indexOf(entry) >= 0 ||
                data.vendorName.toLowerCase().indexOf(entry) >= 0 ||
                data.materialTypes.join().toLowerCase().indexOf(entry) >= 0 ||
                data.progress*100 >= parseInt(entry))
                countMatch++;
            }
            if (countMatch == filterArray.length)
              return true;
            else
              return false;
          };
          this.filter = this.filter.toLowerCase();
          this.dataSource.filter = this.filter;
        }
        else {
          this.uiUtilService.openSnackBar("This project doesn't have any MRFs", 'OK');
        }
        this.isLoading = false;
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
        this.isLoading = false;
      });
  }
}
