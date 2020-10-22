import {
  Component,
  ChangeDetectionStrategy,
  ViewChild,
  TemplateRef,
  ChangeDetectorRef
} from '@angular/core';
import {
  startOfDay,
  endOfDay,
  subDays,
  addDays,
  endOfMonth,
  isSameDay,
  isSameMonth,
  addHours
} from 'date-fns';
import { Subject } from 'rxjs';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import {
  CalendarEvent,
  CalendarEventAction,
  CalendarEventTimesChangedEvent,
  CalendarView
} from 'angular-calendar';
import { MatTableDataSource, MatSort, MatPaginator, MatSelectChange } from '@angular/material';
import { UiUtilsService } from '../../shared/ui-utils.service';
import * as moment from 'moment';
import { TradeAssociation } from '../classes/trade-association';
import { JobTrackService } from '../../job-track/job-track.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ProjectMaster } from '../classes/project-master';
import { MaterialTypeMaster } from '../classes/materialType-master';
import { forEach } from '@angular/router/src/utils/collection';
import { ChecklistItemMaster } from '../classes/checklistItem-master';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';


const colors: any = {
  red: {
    primary: '#6610f2',
    secondary: '#FAE3E3'
  },
  blue: {
    primary: '#1e90ff',
    secondary: '#D1E8FF'
  },
  yellow: {
    primary: '#e3bc08',
    secondary: '#FDF1BA'
  },
  green: {
    primary: '#28a745',
    secondary: '#20c997'
  }
};

export interface Trade {
  value: string;
  viewValue: string;
}

export interface JobEvent<MetaType = any> extends CalendarEvent {
  trades?: Trade[];
  trade?: string;
  materialTrades?: string[];
  subcon?: string;
}

@Component({
  selector: 'app-trade-association',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['trade-association.component.css'],
  templateUrl: 'trade-association.component.html'
})

export class TradeAssociationComponent extends CommonLoadingComponent {
  newTrade: string;
  dataSource: MatTableDataSource<CalendarEvent> = new MatTableDataSource();
  tradesDataSource: MatTableDataSource<TradeAssociation> = new MatTableDataSource();
  materialTypes: MaterialTypeMaster[];
  checklistItems: ChecklistItemMaster[];
  project: ProjectMaster;
  displayedColumns = ['name', 'materialTypes', 'checklistItems'];
  isLoading=false;
  @ViewChild('modalContent')
  modalContent: TemplateRef<any>;

  activeDayIsOpen: boolean = true;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(route: ActivatedRoute, router: Router, private modal: NgbModal, private uiUtilService: UiUtilsService, private jobTrackService: JobTrackService,
    private cdRef:ChangeDetectorRef) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      if (project) {
        this.project = project;
        this.getMaterialTypes()

      }
    });
    //this.uiUtilService.openSnackBar(this.project.id.toString(), 'OK');
  }

  getMaterialTypes() {
    this.jobTrackService.getMaterialTypes(this.project.id).subscribe(
      data => {
        this.materialTypes = data;
        this.getChecklistItems();
        this.getTradeAssociations();
      },
      error => {

        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getChecklistItems() {
    this.jobTrackService.getChecklistItems(this.project.id).subscribe(
      data => {
        this.checklistItems = data;
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getTradeAssociations() {
    this.jobTrackService.getTradeAssociations(this.project.id).subscribe(
      data => {
        this.tradesDataSource = new MatTableDataSource(data);
        if (this.tradesDataSource.data.length > 0) {      
          this.tradesDataSource.paginator = this.paginator;
          this.tradesDataSource.sort = this.sort;
        }
        else {
          this.isLoading = false;
          this.uiUtilService.openSnackBar("No Trade Associations", 'OK');
          this.cdRef.detectChanges();
        }
       
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
      });
      this.isLoading = false;
  }

  onSave() {
    this.isLoading = true;
    this.jobTrackService.updateTradeAssociation(1, this.tradesDataSource.data).subscribe(
      data => {
        this.uiUtilService.openSnackBar('Trade associations has been updated.', "OK");
        this.isLoading = false;
        this.cdRef.detectChanges();
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
        this.isLoading = false;
        this.cdRef.detectChanges();
      }
    );
  }

  unselect(): void {
    console.log('unselect');
    this.jobTrackService.getTradeAssociations(this.project.id).subscribe(
      data=>{
        this.tradesDataSource = new MatTableDataSource(data);
      }
      );
 }
  onselection(value){
    console.log(value);
    this.jobTrackService.getTradeAssociations(this.project.id).subscribe(
    data=>{
      this.tradesDataSource = new MatTableDataSource(data);
    }
    );
  }
}