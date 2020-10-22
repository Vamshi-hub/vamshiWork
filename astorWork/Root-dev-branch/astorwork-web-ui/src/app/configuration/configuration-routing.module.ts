import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { UserMasterComponent } from './user-master/user-master.component';
import { UserDetailsComponent } from './user-details/user-details.component';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { RouteGuardService } from '../route-guard.service';
import { RoleMasterComponent } from './role-master/role-master.component';
import { RoleDetailsComponent } from './role-details/role-details.component';
import { AccessRightResolver } from '../app-resolver.service';
import { GenerateQrCodeComponent } from './generate-qr-code/generate-qr-code.component';
import { ListProjectComponent } from './list-project/list-project.component';
import { ProjectDetailsComponent } from './project-details/project-details.component';

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
      { path: 'import-rfid-tags', component:GenerateQrCodeComponent, resolve: {accessRight: AccessRightResolver}},
      { path: 'project-master', component:ListProjectComponent,resolve:{accessRight:AccessRightResolver}},
      { path: 'project-details/:id', component:ProjectDetailsComponent, resolve:{accessRight:AccessRightResolver}}
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  providers: [AccessRightResolver]
})
export class ConfigurationRoutingModule { }
