export class AuthenticationResult {
    tokenType: string;
    expiresIn: number;
    accessToken: string;
    refreshToken: string;
    userId: number;
    projectID?:number;
}