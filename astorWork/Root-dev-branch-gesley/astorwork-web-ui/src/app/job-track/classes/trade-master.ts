import { MaterialTypeMaster } from "./materialType-master";
import { ChecklistItemMaster } from "./checklistItem-master";

export class TradeMaster {
    id: number;
    name: string;
    materialTypes: MaterialTypeMaster[];
    checklistItems: ChecklistItemMaster[]
}