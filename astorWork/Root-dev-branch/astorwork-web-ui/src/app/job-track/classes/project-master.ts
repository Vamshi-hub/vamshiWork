import { MaterialMaster } from "./material-master";
import { Moment } from "moment";

export class ProjectMaster {
    id: number;
    name: string;
    description: string;
    startDate: Moment;
    endDate: Moment;
    // projectManagerID: number;
    projectManagerName: string;
    materialTypes: string[];
    blocks: string[];
    mrfs: string[];
    country: string;
    timeZoneOffset: number;
    timeZone: string;
}