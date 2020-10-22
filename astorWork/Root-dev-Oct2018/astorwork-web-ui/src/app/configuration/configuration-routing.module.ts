import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserMasterComponent } from './user-master/user-master.component';
import { UserDetailsComponent } from './user-details/user-details.component';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { RouteGuardService } from '../route-guard.service';
import { RoleMasterComponent } from './role-master/role-master.component';
import { RoleDetailsComponent } from './role-details/role-details.component';
import { VendorMasterComponent } from './vendor-master/vendor-master.component';
import { VendorDetailsComponent } from './vendor-details/vendor-details.component';
import { AccessRightResolver } from '../app-resolver.service';
import { LocationMasterComponent } from './location-master/location-master.component';
import { LocationDetailsComponent } from './location-details/location-details.component';
import { StageMasterComponent } from './stage-master/stage-master.component';
import { DetailStageComponent } from './detail-stage/detail-stage.component';
import { GenerateQrCodeComponent } from './generate-qr-code/generate-qr-code.component';
import { ListProjectComponent } from './list-project/list-project.component';
import { ProjectDetailsComponent } from './project-details/project-details.component';
import { SiteMasterComponent } from './site-master/site-master.component';
import { SiteDetailsComponent } from './site-details/site-details.component';
import { NotificationConfigComponent } from './notification-config/notification-config.component';

const routes: Routes = [
  {
    path: 'configuration', component: TopMenuComponent,
    canActivate: [RouteGuardService],
    canActivateChild: [RouteGuardService],
    children: [
      { path: 'user-master', component: UserMasterComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'user-details/:id', component: UserDetailsComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'role-master', component:RoleMasterComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'role-details/:id', component:RoleDetailsComponent, resolve: { accessRight: AccessRightResolver }},
      { path: 'vendor-master', component:VendorMasterComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'vendor-details/:id', component:VendorDetailsComponent, resolve: { accessRight: AccessRightResolver }},
      { path: 'location-master', component:LocationMasterComponent, resolve: { accessRight: AccessRightResolver } },
      { path: 'location-details/:id', component:LocationDetailsComponent, resolve: { accessRight: AccessRightResolver }},
      { path: 'stage-master', component: StageMasterComponent,resolve: { accessRight: AccessRightResolver } },
      { path: 'generate-qr-code', component:GenerateQrCodeComponent, resolve: {accessRight: AccessRightResolver}},
      { path: 'project-master', component:ListProjectComponent,resolve:{accessRight:AccessRightResolver}},
      { path: 'project-details/:id', component:ProjectDetailsComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'stage-master/create', component: DetailStageComponent,resolve:{accessRight:AccessRightResolver} },
      { path: 'site-master', component:SiteMasterComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'site-details/:id', component: SiteDetailsComponent, resolve:{accessRight:AccessRightResolver}},
      { path: 'notification-config', component: NotificationConfigComponent, resolve:{accessRight:AccessRightResolver}}
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [AccessRightResolver]
})
export class ConfigurationRoutingModule { }
