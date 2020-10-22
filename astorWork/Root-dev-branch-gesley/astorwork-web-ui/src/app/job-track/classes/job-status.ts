export class JobStatus {
    delayedJobsCount: number;	
    startedJobsCount: number;	
    completedJobsCount:	number;	
    qCfailedJobsCount: number;	
    scheduledJobsCount:number;
    RequestedJobsCount:number;
    delayedJobsProgress :number;
    startedJobsProgress :number;
    compltedJobsProgress :number;
    qcFailedJobsProgress:number;
    totalQCCount;number;
    projectManager:string;
    projectStartDate:Date;
    projectEndDate:Date;
}
