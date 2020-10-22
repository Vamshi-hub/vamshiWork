export class StageMaster {
    id: number;
    name: string;
    colour: string;
    order: number;
    milestoneId: number;
    isEditable: boolean;
    canIgnoreQC: boolean;
    materialTypes: string;
    materialTypeArray: string[];
    materialCount: number;

    constructor(editable: boolean) {
        this.id = 0;
        this.isEditable = editable;
    }


}