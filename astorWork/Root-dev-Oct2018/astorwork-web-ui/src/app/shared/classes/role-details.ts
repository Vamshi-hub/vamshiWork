import { Moment } from "moment";
import { Module } from "./module";
import { ModulePage } from "./module-page";

export class RoleDetails {
    roleId: number;
    roleName: string;
    defaultPageId: number;
    listofPages: ModulePage[];
}