import { Moment } from "moment";

export class MaterialTrackHistory {
    id: number;
    stageName: string;
    stageStatus: number;
    location: string;
    createdBy: string;
    createdDate: string;
    remarks: string;
    isQCStage: boolean;
    openQCCaseIds: string;
    countQCCase: number;
    countOpenDefect: number;
    countClosedDefect: number;
    countQCDefects: number;
    totalStructChecklistCount: number;
    totalStructPassCount: number;
    totalArchiChecklistCount : number;
    totalArchiPassCount: number;
    structQCLastUpdatedBy: string;
    structQCLastUpdatedDate: Moment;
}