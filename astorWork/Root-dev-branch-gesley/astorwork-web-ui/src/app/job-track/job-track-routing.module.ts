import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { TopMenuComponent } from '../top-menu/top-menu.component';
import { RouteGuardService } from '../route-guard.service';
import { AccessRightResolver } from '../app-resolver.service';
import { TradeAssociationComponent } from './trade-association/trade-association.component';
import { JobSchedulingComponent } from './job-scheduling/job-scheduling.component';
import { JobQCComponent } from './job-qc/job-qc.component';
import { JobTasksComponent } from './job-tasks/job-tasks.component';
import { MaterialTrackResolverService } from '../material-track/material-track-resolver.service';
import { SelectedProjectResolverService } from '../material-track/selected-project-resolver.service';
import { ImportchecklistComponent } from './import-checklist/import-checklist.component';
import { ImportJobscheduleComponent } from './job-scheduling/import-jobschedule/import-jobschedule.component';
import { JobDashboardComponent } from './job-dashboard/job-dashboard.component';
const routes: Routes = [
  {
    path: 'job-tracking', component: TopMenuComponent,
    canActivate: [RouteGuardService],
    canActivateChild: [RouteGuardService],
    resolve: { projects: MaterialTrackResolverService, project: SelectedProjectResolverService },
    children: [
      { path: 'job-association', component: TradeAssociationComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'job-scheduling', component: JobSchedulingComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'job-scheduling/import-jobschedule', component: ImportJobscheduleComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'job-qc', component: JobQCComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'job-tasks', component: JobTasksComponent, resolve:{accessRight:AccessRightResolver}},
      { path:'import-master',component:ImportchecklistComponent, resolve:{accessRight:AccessRightResolver}},
      { path:'job-dashboard',component:JobDashboardComponent, resolve:{accessRight:AccessRightResolver}}
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class JobTrackingRoutingModule { }
