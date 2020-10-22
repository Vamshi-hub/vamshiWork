import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { GlobalErrorComponent } from './global-error/global-error.component';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [
  {
    path: 'global', component: TopMenuComponent,    
    children: [
      { path: 'error', component: GlobalErrorComponent },
    ]
  }
]
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
  declarations: []
})
export class SharedRoutingModule { }
