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
    organisationID: number;
    organisationName: string;
    lastLogin: Moment;
    isActive:boolean;
}