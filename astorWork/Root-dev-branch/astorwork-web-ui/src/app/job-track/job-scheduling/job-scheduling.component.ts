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
  addHours,
  getDate,
  parse
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
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { JobSchedule } from '../classes/job-schedule';
import { ProjectMaster } from '../classes/project-master';
import { MaterialTypeMaster } from '../classes/materialType-master';
import { MaterialTrackService } from '../../material-track/material-track.service';
import { MrfLocation } from '../../material-track/classes/mrf-location';
import { JobTrackService } from '../../job-track/job-track.service';
import { TradeAssociation } from '../classes/trade-association';
import { OrganisationMaster } from '../../material-track/classes/organisation-master';
import { ConfigurationService } from '../../configuration/configuration.service';
import * as moment from 'moment';
import { encodeUriSegment } from '@angular/router/src/url_tree';
import {PageEvent} from '@angular/material/paginator';

const colors: any = {
  fail: {
    primary: '#dc3545',
    secondary: '#FAE3E3'
  },
  blue: {
    primary: '#1e90ff',
    secondary: '#D1E8FF'
  },
  pending: {
    primary: '#e3bc08',
    secondary: '#FDF1BA'
  },
  pass: {
    primary: '#28a745',
    secondary: '#20c997'
  }
};

export interface Job {
  value: string;
  viewValue: string;
}

export interface Zone {
  value: string;
  viewValue: string;
}

export interface MarkingNo {
  value: string;
  viewValue: string;
}

export interface JobCalendarEventTimesChangedEvent<MetaType = any> extends CalendarEventTimesChangedEvent {
  event: JobSchedule<MetaType>;
}

@Component({
  selector: 'app-job-scheduling',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['job-scheduling.component.css'],
  templateUrl: 'job-scheduling.component.html'
})

export class JobSchedulingComponent extends CommonLoadingComponent {
  // MatPaginator Output
  pageEvent: PageEvent;
  pageIndex: number;
  pageSize: number=10;
  lastMaterialIndex:number=0;
  @ViewChild('modalContent')
  modalContent: TemplateRef<any>;
  project: ProjectMaster;
  jobsDataSource: MatTableDataSource<JobSchedule> = new MatTableDataSource();
  jobscheduledata:JobSchedule[];
  filterJOBS = '';
  jobid= '';
  subconsDataSource: MatTableDataSource<OrganisationMaster> = new MatTableDataSource();
  displayedColumns = ['level', 'materialType', 'zone', 'markingNo', 'trade', 'subcon', 'plannedStartDate', 'plannedEndDate', 'status'];

  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;

  viewDate: Date = new Date();

  modalData: {
    action: string;
    event: JobSchedule;
  };

  actions: CalendarEventAction[] = [
    {
      label: '<i class="fa fa-fw fa-pencil"></i>',
      onClick: ({ event }: { event: JobSchedule }): void => {
        this.handleEvent('Edited', event);
      }
    },
    {
      label: '<i class="fa fa-fw fa-times"></i>',
      onClick: ({ event }: { event: JobSchedule }): void => {
        this.jobs = this.jobs.filter(iEvent => iEvent !== event);
        this.handleEvent('Deleted', event);
      }
    }
  ];

  refresh: Subject<any> = new Subject();
  materialType: string ='';
  materialTypes: string[];
  markingNos: MarkingNo[];
  tradeAssociations: TradeAssociation[];
  trades: Set<string>;
  subcons: OrganisationMaster[];
  subcon: string='';
  subconsList:string[];
  jobs: JobSchedule[];
  block: string='';
  blocks: string[];
  level: string='';
  levels: string[];

  mrfLocations: MrfLocation[];
  minDate = new Date();

  activeDayIsOpen: boolean = true;
  @ViewChild(MatSort) sort: MatSort;
  @ViewChild(MatPaginator) paginator: MatPaginator;

  constructor(private modal: NgbModal, private materialTrackService: MaterialTrackService, private uiUtilService: UiUtilsService, private configurationService: ConfigurationService,
    private jobTrackService: JobTrackService, private cdRef:ChangeDetectorRef, route: ActivatedRoute, router: Router) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();
    this.route.parent.data.subscribe(async (data: { project: ProjectMaster }) => {
      const project = await (data.project);
      
      this.subcon = '';
      if (project) {
        this.project = project;
        this.route.paramMap.subscribe((params: Params) => {
            this.jobid = params.get('qcids'); 
          this.filterJOBS = params.get('filter');
        });
        this.blocks = this.project.blocks;
        //this.block = this.blocks[0];
        this.getLevels();
        this.getMaterialTypes();
        this.getSubcons();
        this.getTradeAssociations();
        this.getJobs();
      }

      
      this.materialType = '';//this.materialTypes[0];
    });
  }

  getJobs(){
    this.isLoading = true;
  //  this.jobTrackService.getJobs(this.project.id, this.block, this.level, this.materialType, this.subcon, lastMaterialIndex).subscribe(
    this.jobTrackService.getScheduleJobs(this.project.id,this.lastMaterialIndex,this.pageSize,this.block, this.level, this.materialType, this.subcon,).subscribe(
      data => {     
        this.jobsDataSource.data=this.jobsDataSource.data.concat(data) 
        this.jobscheduledata=data;
        if(this.jobid != null)
          this.filterDataBasedOnId(this.jobid)
        else if(this.filterJOBS !=null)
          this.filterDataBasedOnQueryString(this.filterJOBS);
       
        if (this.jobsDataSource.data.length > 0) {
          this.jobsDataSource.data.forEach(item => {
            this.setCalendarItemStartDate(item);
            this.setCalendarItemEndDate(item);
            
            item.title = item.tradeName;
            item.draggable = true;
            
            this.setCalendarItemStatus(item);
            this.setCalendarItemColour(item);
            
            item.status = item.status.replace(/_/g, ' ');
            item.draggable = false;
          })
        }

        setTimeout(() => {
          this.jobsDataSource.sort = this.sort;
          this.jobsDataSource.paginator = this.paginator;
        });
      
        this.isLoading = false;
        this.cdRef.detectChanges();
      },
      error => {
        this.isLoading = false;
        this.cdRef.detectChanges();
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
    
  }

  setCalendarItemStartDate(item){
    if (item.start != null)
      item.start = new Date(item.start);
    else if (item.end != null)
      item.start = new Date(item.end);
  }

  setCalendarItemEndDate(item){
    if (item.end != null)
      item.end = new Date(item.end);
    else if (item.start != null)
      item.end = new Date(item.start);
  }

  setCalendarItemStatus(item){
    if(item.status=='Job_started_by_Subcon')
      item.status='Ongoing';
  }

  setCalendarItemColour(item){
    if (item.status == 'Job_not_assigned' || item.status == 'Job_not_started_by_Subcon' || item.status == 'Job_delayed' || item.status == 'Ongoing')
      item.color = colors.blue;
    else if (item.status == 'Job_completed_by_Subcon' || item.status == 'QC_passed_by_Maincon' || item.status == 'QC_routed_to_RTO' || item.status == 'QC_accepted_by_RTO')
      item.color = colors.pending;
    else if (item.status == 'QC_failed_by_Maincon' || item.status == 'QC_rectified_by_Subcon' || item.status == 'QC_rejected_by_RTO')
      item.color = colors.fail;
    else if (item.status == 'All_QC_passed')
      item.color = colors.pass;
  }

  getSubcons(){
    this.configurationService.getOrganisations().subscribe(
      data => {
        this.subconsDataSource = new MatTableDataSource(data);
        if (this.subconsDataSource.data.length > 0) {
          this.subcons = data.filter(s => s.organisationType == 2);
          this.subconsList = new Array(this.subcons.map((subcon) => subcon.name))[0];
          //this.getTradeAssociations();
         // this.getJobs(1);
        }
        else 
          this.uiUtilService.openSnackBar("No Organisations", 'OK');
        
        this.isLoading = false;
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getLevels(){
    this.jobTrackService.getLocationForJobSchedule(this.project.id, this.block).subscribe(
      data => {
        if (data != null){
          this.mrfLocations = data;
          
          if (this.mrfLocations[0] != null){
            this.levels = new Array(this.mrfLocations.map((location) => location.level))[0];
         //   this.level = this.levels[0];
            this.cdRef.detectChanges();
           // this.getMaterialTypes();
          }
        }
      },
      error => {
        this.isLoading = false;
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getMaterialTypes() {
    this.jobTrackService.getMaterialTypes(this.project.id).subscribe(
      data => {
        this.materialTypes = new Array(data.map((materialType) => materialType.name))[0];
        //this.materialType = this.materialTypes[0];
        //this.getSubcons();
        this.cdRef.detectChanges();
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
      }
    );
  }

  getTradeAssociations() {
    this.jobTrackService.getTradeAssociations(this.project.id).subscribe(
      data => {
        this.tradeAssociations = data;
        this.trades = new Set(this.tradeAssociations.map((tradeAssociation) => tradeAssociation.name));
        this.isLoading = false;
        //this.getMaterialTypes();
        this.cdRef.detectChanges();
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
      });
  }

  dayClicked({ date, events }: { date: Date; events: JobSchedule[] }): void {
    if (isSameMonth(date, this.viewDate)) {
      this.viewDate = date;
      if (
        (isSameDay(this.viewDate, date) && this.activeDayIsOpen === true) ||
        events.length === 0
      ) {
        this.activeDayIsOpen = false;
      } else {
        this.activeDayIsOpen = true;
      }
    }
  }

  eventTimesChanged({
    event,
    newStart,
    newEnd
  }: JobCalendarEventTimesChangedEvent): void {
    event.start = newStart;
    event.end = newEnd;
    event.isUpdated = true;
    //this.handleEvent('Dropped or resized', event);
    this.refresh.next();
  }

  handleEvent(action: string, event: JobSchedule): void {
    this.modalData = { event, action };
    this.modal.open(this.modalContent, { size: 'lg' });
  }

  addJobSchedule(): void {
    this.refresh.next();
  }

  onSave() {
    this.isLoading = true;
    this.jobTrackService.saveJobs(1, this.jobsDataSource.data.filter(j => j.isUpdated)).subscribe(
      data => {
        this.uiUtilService.openSnackBar('Job schedule has been updated.', "OK");
        this.isLoading = false;
        this.getJobs();
        this.cdRef.detectChanges();
      },
      error => {
        this.uiUtilService.openSnackBar(error, 'OK');
        this.isLoading = false;
        this.cdRef.detectChanges();
      }
    );
    
  }

  checklist(): void{
    this.router.navigateByUrl('/job-tracking/job-tasks;status='+this.modalData.event.status);
  }

  onBlockChanged(event: MatSelectChange) {
    if (event.value != undefined && event.value!='') {
      this.block = event.value;
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
      this.getLevels();
      this.getJobs();
      this.cdRef.detectChanges();
    }
    else if(event.value==''){
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
       this.getJobs();
      this.cdRef.detectChanges();
    }
    else 
      this.uiUtilService.openSnackBar('Block not selected!', 'OK');
  }

  onLevelChanged(event: MatSelectChange) {
    if (event.value != undefined && event.value!='' ) {
      this.level = event.value;
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
      this.getJobs();
      this.cdRef.detectChanges();
    }
    else if(event.value==''){
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
       this.getJobs();
      this.cdRef.detectChanges();
    }
    else 
      this.uiUtilService.openSnackBar('Level not selected!', 'OK');
  }

  onMaterialTypeChanged(event: MatSelectChange) {
    if (event.value != undefined && event.value!='') {
      this.materialType = event.value;
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
      this.getJobs();
      this.cdRef.detectChanges();
    }
    else if(event.value==''){
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
       this.getJobs();
      this.cdRef.detectChanges();
    }
    else
      this.uiUtilService.openSnackBar('Material Type not selected!', 'OK');
  }

  onSubconChanged(event: MatSelectChange) {
    if (event.value != undefined && event.value!='') {
      this.subcon = event.value;
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
      this.getJobs();
      this.cdRef.detectChanges();
    }
    else if(event.value==''){
      this.jobsDataSource=new MatTableDataSource();
      this.lastMaterialIndex=0;
       this.getJobs();
      this.cdRef.detectChanges();
    }
    else 
      this.uiUtilService.openSnackBar('Material Type not selected!', 'OK');
  }
  
  filterDataBasedOnId(Id: string) {
    this.jobsDataSource.data =this.jobsDataSource.data.filter(j=>j.id==Id) 
  }

  filterDataBasedOnQueryString(filterkey: string) {
    this.jobsDataSource.data =this.jobsDataSource.data.filter(j=>j.status===filterkey)    
  }

  onFilterKeyIn(event: KeyboardEvent) {
    var filterValue = (<HTMLInputElement>event.target).value;
    filterValue = (filterValue && filterValue.trim()) || '';
    filterValue = filterValue.toLowerCase(); // MatTableDataSource defaults to lowercase matches
    this.jobsDataSource.filter = filterValue;
    if (filterValue == null)
      this.getJobs();
  }

  // onPage(event: PageEvent){
  //   this.pageIndex = event.pageIndex;
  //   this.pageSize = event.pageSize;
  //   this.getJobs(this.pageIndex * this.pageSize);
  // }
  onScroll(){
    this.lastMaterialIndex=this.lastMaterialIndex+10;
    this.getJobs();
  }
}
