
import {switchMap} from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { MaterialTrackService } from './material-track.service';
import { ProjectMaster } from './classes/project-master';
import { Observable, of, throwError } from 'rxjs';
import { UiUtilsService } from '../shared/ui-utils.service';
import { UserDetails } from './classes/user-details';

@Injectable()
export class SelectedProjectResolverService implements Resolve<ProjectMaster> {

  project: ProjectMaster;

  constructor(private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ProjectMaster> {
    // this.uiUtilService.openDialog(SpinnerDlgComponent, null);
    const projectId = window.localStorage.getItem('project_id');


    if (projectId) {
      return this.materialTrackService.getProjectInfo(+projectId, "");
    }
    else {
      const userId = Number(window.localStorage.getItem('user_id'));

      return this.materialTrackService.getUserDetails(userId).pipe(switchMap(
        data => {
          if (data.projectID > 0)
            return this.materialTrackService.getProjectInfo(+data.projectID, "");
          else
            return of(null);
        }
      ))
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
