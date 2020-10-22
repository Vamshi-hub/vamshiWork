import { Moment } from 'moment';
import { InProgress } from './inProgress';
import { OverallProgress } from './overallProgress';

export class OverallAndInProgress {
    inProgressList: InProgress[];
    overallProgressList: OverallProgress[];
}
