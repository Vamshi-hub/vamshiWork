import { UserMaster } from "../../shared/classes/user-master";
import { LocationMaster } from "../../shared/classes/location-master";

export class OrganisationMaster{
    id: number;
    name: string;
    cycleDays: number;
    contactPeople: UserMaster[];
    locations: LocationMaster[];
    description: string;
    organisationType: number;
    organisationTypeName: string;
}