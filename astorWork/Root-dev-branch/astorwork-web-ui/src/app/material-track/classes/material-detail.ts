import { MaterialTrackHistory } from './material-track-history';
import { Moment } from 'moment';
import { MaterialDrawing } from './material-drawing';

export class MaterialDetail {
    id: number;
    block: string;
    level: string;
    zone: string;
    markingNo: string;
    materialType: string;
    organisationName: string;
    remarks: string;
    orderDate: Moment;
    expectedDeliveryDate: Moment;
    castingDate: Moment;
    trackerType: string;
    trackerTag: string;
    trackerLabel: string;
    trackingHistory: MaterialTrackHistory[];
    drawing: MaterialDrawing;
    area:string;
    length:number;
    dimensions:number;
}