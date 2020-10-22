import { Moment } from "moment";

export class MrfMaster {
    id: number;
    mrfNo: string;
    orderDate: Moment;
    plannedCastingDate: Moment;
    vendorName: string;
    block: string;
    level: string;
    zone: string;
    materialTypes: string[];

    progress: number;
}
