import { Moment } from "moment";
import { MaterialMaster } from "./material-master";

export class BIMSyncSession {
    id: number;
    bimModelId: string;
    personName: string;
    countSyncedMaterials: number;
    countUnsyncedMaterials: number;
    syncedMaterials: MaterialMaster[];
    unsyncedMaterials: MaterialMaster[];
    videoURL: string;
    syncTime: Moment;
}