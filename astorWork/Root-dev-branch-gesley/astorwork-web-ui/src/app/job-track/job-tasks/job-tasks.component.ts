import {
  Component,
  ChangeDetectionStrategy,
  ViewChild,
  TemplateRef
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

import { UiUtilsService } from '../../shared/ui-utils.service';

import { QCPhoto } from '../classes/qc-photo';
import * as moment from 'moment';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { CommonLoadingComponent } from '../../shared/common-loading/common-loading.component';
import { JobTasksSlideShowComponent } from './slide-show/slide-show.component';

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

export interface Job {
  value: string;
  viewValue: string;
}

export interface Level {
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

export interface JobEvent<MetaType = any> extends CalendarEvent {
  level: string;
  zone: string;
  jobs?: Job[];
  job?: string;
  materialJobs?: string[];
  subcon?: string;
  disabled?: boolean;
  actualStart?: Date;
  actualEnd?: Date;
  status: String;
}

@Component({
  selector: 'app-job-tasks',
  changeDetection: ChangeDetectionStrategy.OnPush,
  styleUrls: ['job-tasks.component.css'],
  templateUrl: 'job-tasks.component.html'
})

export class JobTasksComponent extends CommonLoadingComponent {
  @ViewChild('modalContent')
  modalContent: TemplateRef<any>;

  view: CalendarView = CalendarView.Month;

  CalendarView = CalendarView;

  viewDate: Date = new Date();

  modalData: {
    action: string;
    event: CalendarEvent;
  };

  refresh: Subject<any> = new Subject();

  qcPhoto: QCPhoto[] = [
    {
      id: 1,
      url: 'https://astorwork.blob.core.windows.net/dev-tenant-repo/sink-damage-1.JPG',
      remarks: '',
      isOpen: true,
      createdBy: 'Benjamin',
      createdDate:  moment(new Date()),
      countPhotos: 1
    },
    {
      id: 2,
      url: 'https://astorwork.blob.core.windows.net/dev-tenant-repo/sink-fixed-2.jpg',
      remarks: '',
      isOpen: true,
      createdBy: 'Benjamin',
      createdDate:  moment(new Date()),
      countPhotos: 1
    }
  ]

  levelsList: Level[] = [
    {value: '1', viewValue: '1'},
    {value: '2', viewValue: '2'},
    {value: '3', viewValue: '3'},
    {value: '4', viewValue: '4'},
    {value: '5', viewValue: '5'},
    {value: '6', viewValue: '6'},
    {value: '7', viewValue: '7'},
    {value: '8', viewValue: '8'},
    {value: '9', viewValue: '9'},
    {value: '10', viewValue: '10'}
  ]

  zonesList: Zone[] = [
    {value: '1', viewValue: '1'},
    {value: '2', viewValue: '2'},
    {value: '3', viewValue: '3'},
    {value: '4', viewValue: '4'}
  ]

  markingNosList: MarkingNo[] = [
    {value: '2R2LD', viewValue: '2R2LD'},
    {value: '2R1MB', viewValue: '2R1MB'},
    {value: '2R1KC-PBU', viewValue: '2R1KC-PBU'},
    {value: '3RLD', viewValue: '3RLD'},
    {value: '3RMB1A-PBU', viewValue: '3RMB1A-PBU'},
    {value: '3RB2A-PBU', viewValue: '3RB2A-PBU'},
    {value: '3RKC', viewValue: '3RKC'}
  ]

  events: JobEvent[] = [
    {
      start: startOfDay(new Date()),
      level: '8',
      zone: '1', 
      title: '2R2LD',
      color: colors.red,
      jobs: [
        {value: 'MEP Installation', viewValue: 'MEP Installation'},
        {value: 'Waterproofing', viewValue: 'Waterproofing'},
        {value: 'Floor Screeding', viewValue: 'Floor Screeding'},
        {value: 'Window Frame Installation', viewValue: 'Window Frame Installation'},
        {value: 'Door Frame Installation', viewValue: 'Door Frame Installation'}
      ],
      job: 'Floor Screeding',
      subcon: 'EZ Flooring Inc',
      status:'Not Started'
    },
    {
      start: addHours(startOfDay(new Date()), 2),
      end: new Date(),
      level: '8',
      zone: '1', 
      title: '2R1MB',
      jobs: [
        {value: 'MEP Installation', viewValue: 'MEP Installation'},
        {value: 'Waterproofing', viewValue: 'Waterproofing'},
        {value: 'Floor Screeding', viewValue: 'Floor Screeding'},
        {value: 'Window Frame Installation', viewValue: 'Window Frame Installation'},
        {value: 'Door Frame Installation', viewValue: 'Door Frame Installation'},
        {value: 'Wardrobe Installation', viewValue: 'Wardrobe Installation'},
      ],
      job: 'Wardrobe Installation',
      color: colors.blue,
      subcon: 'Install Express Corp',
      status:'Not Started',
      resizable: {
        beforeStart: true,
        afterEnd: true
      },
      draggable: true
    },
    {
      start: subDays(endOfMonth(new Date()), 3),
      end: addDays(endOfMonth(new Date()), 3),
      level: '8',
      zone: '4', 
      title: '3RKC',
      color: colors.yellow,
      jobs: [
        {value: 'MEP Installation', viewValue: 'MEP Installation'},
        {value: 'Waterproofing', viewValue: 'Waterproofing'},
        {value: 'Floor Screeding', viewValue: 'Floor Screeding'},
        {value: 'Window Frame Installation', viewValue: 'Window Frame Installation'},
      ],
      job: 'Window Frame Installation',
      subcon: 'Install Express Corp',
      status:'Not Started',
      allDay: false
    }
  ];

  activeDayIsOpen: boolean = true;
  slideShowData: SlideShowData;
  status = "";
  level: String;

  constructor(private modal: NgbModal, private uiUtilService: UiUtilsService, route: ActivatedRoute, router: Router) {
    super(route, router);
  }

  ngOnInit() {
    super.ngOnInit();

    this.route.paramMap.subscribe((params: Params) => {
      this.status = params.get('status');
    });

    if (this.status=="Passed")
      this.level = "6"
    else if (this.status=="Failed")
      this.level = "7"
    else 
      this.level = "8"
  }

  getPhoto() {
    this.isLoading = true;
    
    this.slideShowData = new SlideShowData();
    this.slideShowData.qcOpenPhotos = this.qcPhoto.filter(p => p.isOpen);
    this.slideShowData.defectID = 1;
    this.slideShowData.remarks = "";
    this.slideShowData.qcClosePhotos = this.qcPhoto.filter(p => p.isOpen == false);
    this.isLoading = false;
    this.uiUtilService.openDialog(JobTasksSlideShowComponent, this.slideShowData, true);
  }

  OnCancel() {
    this.router.navigateByUrl('/job-tracking/job-scheduling');
  }
}

export class SlideShowData {
  defectID: number;
  remarks: string; keys
  qcOpenPhotos: QCPhoto[];
  qcClosePhotos: QCPhoto[];
}