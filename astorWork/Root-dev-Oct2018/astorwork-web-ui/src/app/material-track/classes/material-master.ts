import { Moment } from "moment";

export class MaterialMaster {
    id: number;
    block: string;
    level: string;
    zone: string;
    markingNo: string;
    mrfNo: string;
    materialType: string;
    stageColour: string;
    stageName: string;
    modelUrn: string;
    elementId: number;
    expectedDeliveryDate:Moment;
    stageOrder: number;
    deliveryStageOrder: number;
}