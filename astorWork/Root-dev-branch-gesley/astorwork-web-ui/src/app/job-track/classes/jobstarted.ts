import { Moment } from "moment";


export class Jobstarted{
    tradeName: string;
    markingNo: string;
    subConName: string;
    startDate: Date;
    completedDate:  Date;
    checklistName:string;
    qcFailedBy:string;
    lastModifiedDate:Date;
    expectedStartdate:Date;
    actualStartDate:Date;
    plannedStartDate:Date;
    actualEndDate:Date;
    plannedEndDate:Date;
    qcStartDate:Date;
    qcEndDate:Date;
    lastchecklistupdatedby:string;
    countPhotos: number;
    id: number;
    isOpen:string;
    checklistID:number;
    uRL:string
    status:string;
    totalQCCount;number;
    block:string;
    level:string;
    zone:string;
    type:string;
    materialType:string;
    checklistStatus:string;
    stageName:string;
}