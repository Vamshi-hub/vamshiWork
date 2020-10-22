import { Moment } from "moment";
import { Module } from "./module";
import { ModulePage } from "./module-page";

export class RoleDetails {
    id: number;
    name: string;
    defaultPageID: number;
    pages: ModulePage[];
}