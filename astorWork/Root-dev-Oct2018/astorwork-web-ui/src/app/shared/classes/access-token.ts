import { Moment } from "moment";
import { UserRole } from "./user-role";

export class AccessToken {
    sub: string;
    nbf: number;
    exp: number;
    role: UserRole;
    personName: string;
    vendorId: number;
}

export class ForgeToken {
    access_token: string;
    token_type: string;
    expires_in: number;
}

export class PowerBIAuthResult {
    token_type: string;
    scope: string;
    expires_in: number;
    expires_on: number;
    not_before: number;
    resource: string;
    access_token: string;
    refresh_token: string;
    id_token: string;
}

export class PowerBIEmbedToken{
    token: string;
    tokenId: string;
    expiration: Moment;
}