import { Moment } from "moment";

export class QCCase {
    id: number;
    material: string;
    job: string;
    isOpen: string;
    createdBy: string;
    createdDate: Moment;
    updatedBy: string;
    updatedDate: Moment;
    countClosedDefects: number;
    countOpenDefects: number;
    duration: string;
    progress: number;
}