import { Moment } from "moment";

export class UserRole {
    roleId: number;
    userId: number;
    expiryTime: Moment;
    pageAccessRights: AccessRight[];
    defaultPage: string;
}

export class AccessRight {
    url: string;
    right: number;
}