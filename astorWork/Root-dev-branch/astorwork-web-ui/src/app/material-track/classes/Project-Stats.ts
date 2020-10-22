import { Moment } from 'moment';

export class ProjectStats {
    installedMaterialsCount: number;	
    totalMaterialsCount: number;	
    installedMaterialsProgress:	number;	
    deliveredMaterialsCount: number;	
    requestedMaterialsCount: number;	
    deliveredMaterialsProgress:	number;	
    completedMRFCount: number;	
    totalMRFCount: number;
    completedMRFProgress: number;
    qcFailedCount: number;
    qcTotalCount: number;
    qcFailedProgress: number
    readytoProject:number;
    readyToDeliveredMaterialsProgress:number;
    readyToDeliveredMaterialsCount:number;
    projectManager:string;
    projectStartDate:Date;
    projectEndDate:Date;

}