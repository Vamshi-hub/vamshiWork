import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ChangePasswordComponent } from './change-password/change-password.component';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { UserDetailsComponent } from '../configuration/user-details/user-details.component';

const routes: Routes = [
  {
    path: 'user-account', component: TopMenuComponent,
    children: [
      { path: 'change-password', component: ChangePasswordComponent },
      { path: ':id', component: UserDetailsComponent }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class UserAccountRoutingModule { }
