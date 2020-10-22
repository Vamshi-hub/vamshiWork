import { Moment } from "moment";

export class MaterialMaster {
    id: number;
    markingNo: string;
    block: string;
    level: string;
    zone: string;
    mrfNo: string;
    materialType: string;
    stageColour: string;
    stageName: string;
    forgeModelURN: string;
    forgeElementID: number;
    expectedDeliveryDate:Moment;
    stageOrder: number;
    deliveryStageOrder: number;
    openQCCaseID: number;
    qcStatus: number;
}

export class MaterialMasterLHL{
    block: string;
    level: string;
    zone: string;
    markingNo: string;
    gridLine: string;
    materialType: string;
    mrfNo: string;
    plannedProductionDate: Moment;
    actualProductionDate: Moment;
    plannedDeliveryDate: Moment;
    actualDeliveryDate: Moment;
    plannedInstallationDate: Moment;
    actualInstallationDate: Moment;
    qcCase: number;
}