import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ListMaterialComponent } from './list-material/list-material.component';
import { MaterialDetailsComponent } from './material-details/material-details.component';
import { CreateMrfComponent } from './create-mrf/create-mrf.component';
import { ListMrfComponent } from './list-mrf/list-mrf.component';
import { MaterialTrackResolverService } from './material-track-resolver.service';
import { ListBimSyncComponent } from './list-bim-sync/list-bim-sync.component';
import { BimSyncDetailsComponent } from './bim-sync-details/bim-sync-details.component';
import { SelectedProjectResolverService } from './selected-project-resolver.service';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardConsultantComponent } from './dashboard-consultant/dashboard-consultant.component';
import { DashboardMainconComponent } from './dashboard-maincon/dashboard-maincon.component';
import { DashboardAlecComponent } from './dashboard-alec/dashboard-alec.component';
import { RouteGuardService } from '../route-guard.service';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { AccessRightResolver } from '../app-resolver.service';
import { QcDetailsComponent } from './qc-details/qc-details.component';
import { ListDefectComponent } from './list-defect/list-defect.component';
import { ImportMaterialComponent } from './import-material/import-material.component';
import { ImportFileComponent } from './import-file/import-file.component';
import { ListReportsComponent } from './list-reports/list-reports.component';
import { PowerbiViewerComponent } from './powerbi-viewer/powerbi-viewer.component';
import { OrganisationMasterComponent } from '../configuration/organisation-master/organisation-master.component';
import { OrganisationDetailsComponent } from '../configuration/organisation-details/organisation-details.component';
import { LocationMasterComponent } from '../configuration/location-master/location-master.component';
import { LocationDetailsComponent } from '../configuration/location-details/location-details.component';
import { StageMasterComponent } from '../configuration/stage-master/stage-master.component';
import { DetailStageComponent } from '../configuration/detail-stage/detail-stage.component';
import { SiteMasterComponent } from '../configuration/site-master/site-master.component';
import { SiteDetailsComponent } from '../configuration/site-details/site-details.component';
import { NotificationConfigComponent } from '../configuration/notification-config/notification-config.component';
import { MaterialQcComponent } from './material-qc/material-qc.component';
import { MaterialDashboardComponent } from './material-dashboard/material-dashboard.component';
import { MaterialstagedashboardComponent } from './materialstagedashboard/materialstagedashboard.component';
const routes: Routes = [
  {
    path: 'material-tracking', component: TopMenuComponent,
    canActivate: [RouteGuardService],
    canActivateChild: [RouteGuardService],
    resolve: { projects: MaterialTrackResolverService, project: SelectedProjectResolverService },
    children: [
      { path: 'materials', component: ListMaterialComponent },
      { path: 'materials/:id', component: MaterialDetailsComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'mrfs', component: ListMrfComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'mrfs/create', component: CreateMrfComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'bim-syncs', component: ListBimSyncComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'bim-syncs/:id', component: BimSyncDetailsComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'dashboard', component: DashboardComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'dashboard-consultant', component: DashboardConsultantComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'dashboard-maincon', component: DashboardMainconComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'dashboard-alec', component: DashboardAlecComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'qc-cases/:id', component: QcDetailsComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'qc-defects', component: ListDefectComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'import-material', component: ImportMaterialComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'import-file', component: ImportFileComponent, resolve: { accessRight: AccessRightResolver } },
      {
        path: 'dashboard-alec', component: DashboardAlecComponent, resolve: { accessRight: AccessRightResolver },
        children: [
          {
            path: ':guid', component: PowerbiViewerComponent, resolve: { accessRight: AccessRightResolver }
          }
        ]
      },
      { path: 'organisation-master', component:OrganisationMasterComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'organisation-details/:id', component:OrganisationDetailsComponent, resolve: { accessRight: AccessRightResolver }},
      { path: 'location-master', component:LocationMasterComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'location-details/:id', component:LocationDetailsComponent, resolve: { accessRight: AccessRightResolver }},
      { path: 'stage-master', component: StageMasterComponent,resolve: { accessRight: AccessRightResolver } },
      { path: 'stage-master/create', component: DetailStageComponent,resolve:{accessRight:AccessRightResolver} },
      { path: 'site-master', component:SiteMasterComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'site-details/:id', component: SiteDetailsComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'notification-config', component: NotificationConfigComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'material-qc', component: MaterialQcComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'material-dashboard', component: MaterialstagedashboardComponent, resolve:{accessRight:AccessRightResolver}},
      // { path: 'materialstagedashboard', component: MaterialstagedashboardComponent, resolve:{accessRight:AccessRightResolver}},


    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [MaterialTrackResolverService, SelectedProjectResolverService]
})
export class MaterialTrackRoutingModule { }
