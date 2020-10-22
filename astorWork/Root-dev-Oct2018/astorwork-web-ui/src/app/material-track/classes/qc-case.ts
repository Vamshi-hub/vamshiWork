import { Moment } from "moment";

export class QCCase {
    id: number;
    caseName: string;
    isOpen: string;
    createdBy: string;
    createdDate: Moment;
    updatedBy: string;
    updatedDate: Moment;
    countClosedDefects: number;
    countOpenDefects: number;
    markingNo: string;
    stageName: string;
    duration: string;
    progress: number;
    stageAuditId: number;
}