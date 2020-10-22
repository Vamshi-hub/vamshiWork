import { Moment } from "moment";

export class QCDefect{
    id: number;
    remarks: string;
    statusCode:number;
    isOpen: string;
    createdBy: string;
    createdDate:  Moment;
    updatedBy: string;
    updatedDate:  Moment;
    countPhotos: number;
    caseID: number;
    
}