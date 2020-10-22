import { MaterialTypeMaster } from '../classes/materialType-master';

import {
    CalendarEvent
  } from 'angular-calendar';
import { Moment } from 'moment';

export interface JobSchedule<MetaType = any> extends CalendarEvent {
    ID: number;
    level: string;
    materialType: string;
    markingNo: string;
    tradeID: number;
    tradeName: string;
    subConID: number;
    subConName: string;
    actualStartDate: Moment;
    actualEndDate: Moment;
    statusCode:number;
    status: string;
    isUpdated: boolean;
  }