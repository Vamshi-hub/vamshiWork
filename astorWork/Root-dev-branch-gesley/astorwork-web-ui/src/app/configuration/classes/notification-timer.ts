import { Moment } from "moment";

export class NotificationTimer
{
    id: number;
    code: number;
    notificationName: string;
    description: string;
    triggerTime: string;
    timer: Moment;
    enabled: boolean;
    siteId: number;
    projectId: number;
}