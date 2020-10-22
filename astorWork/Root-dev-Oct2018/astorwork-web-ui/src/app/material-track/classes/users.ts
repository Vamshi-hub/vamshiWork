import { Moment } from 'moment';

export class Users {
    userID: number;
    personName: string;
    email: string;
    userName: string;
    roleID: number;
    role: string;
    projectID: number;
    projectName: string;
    siteID: number;
    site: string;
    vendorID: number;
    vendorName: string;
    lastLogin: Moment;
    isActive:boolean;
}