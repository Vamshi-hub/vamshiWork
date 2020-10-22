import { Moment } from "moment";

export class QCPhoto{
    id: number;
    url: string;
    remarks: string;
    isOpen: boolean;
    createdBy: string;
    createdDate:  Moment;
    countPhotos:number;
}