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
import { RouteGuardService } from '../route-guard.service';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { AccessRightResolver } from '../app-resolver.service';
import { QcDetailsComponent } from './qc-details/qc-details.component';
import { ListDefectComponent } from './list-defect/list-defect.component';
import { ImportMaterialComponent } from './import-material/import-material.component';
import { ListReportsComponent } from './list-reports/list-reports.component';
import { PowerbiViewerComponent } from './powerbi-viewer/powerbi-viewer.component';

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
      { path: 'qc-cases/:id', component: QcDetailsComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'qc-defects/:id', component: ListDefectComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'import-material', component: ImportMaterialComponent, resolve: { accessRight: AccessRightResolver } },
      {
        path: 'list-reports', component: ListReportsComponent, resolve: { accessRight: AccessRightResolver },
        children: [
          {
            path: ':guid', component: PowerbiViewerComponent, resolve: { accessRight: AccessRightResolver }
          }
        ]
      },

    ]
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [MaterialTrackResolverService, SelectedProjectResolverService]
})
export class MaterialTrackRoutingModule { }
