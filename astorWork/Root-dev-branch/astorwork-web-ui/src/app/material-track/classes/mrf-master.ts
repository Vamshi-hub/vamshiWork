import { Moment } from "moment";

export class Mrf {
    id: number;
    mrfNo: string;
    orderDate: Moment;
    plannedCastingDate: Moment;
    expectedDeliveryDate: Moment;
    organisationName: string;
    block: string;
    level: string;
    zone: string;
    materialTypes: string[];

    progress: number;
}
