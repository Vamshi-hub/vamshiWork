import { MaterialTypeMaster } from "./materialType-master";
import { ChecklistItemMaster } from "./checklistItem-master";

export class TradeAssociation {
    id: number;
    name: string;
    materialTypes: string[];
    jobStartedMaterialTypes: string[]
    checklistItems: string[];
}