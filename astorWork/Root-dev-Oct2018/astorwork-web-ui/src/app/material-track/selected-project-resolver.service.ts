import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { MaterialTrackService } from './material-track.service';
import { ProjectMaster } from './classes/project-master';
import { Observable, of, throwError } from 'rxjs';

import 'rxjs/add/operator/switchMap';
import 'rxjs/add/operator/catch';
import { UiUtilsService } from '../shared/ui-utils.service';
import { UserDetails } from './classes/user-details';


@Injectable()
export class SelectedProjectResolverService implements Resolve<ProjectMaster> {
  
  project:ProjectMaster;

  constructor(private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ProjectMaster> {
    // this.uiUtilService.openDialog(SpinnerDlgComponent, null);
    const projectId = window.localStorage.getItem('project_id');
    

    if (projectId) {
      return this.materialTrackService.getProjectInfo(+projectId);
    }
    else{
      const userId = Number(window.localStorage.getItem('user_id'));
      
      return this.materialTrackService.getUserDetails(userId).switchMap(
        data => {
          return this.materialTrackService.getProjectInfo(+data.projectID);          
        }
      )
    }
    /*
    .switchMap(projects => {
      if (projects && (projects as ProjectMaster[]).length > 0) {
        return this.materialTrackService.getProjectInfo(projects[0].id);
      }
      else
        return throwError('Project not found');
    }).catch(error => throwError(error));
    */
  }

}
