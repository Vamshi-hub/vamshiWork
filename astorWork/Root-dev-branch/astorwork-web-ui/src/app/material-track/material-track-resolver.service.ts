import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { MaterialTrackService } from './material-track.service';
import { ProjectMaster } from './classes/project-master';
import { Observable, of, throwError } from 'rxjs';



import { UiUtilsService } from '../shared/ui-utils.service';
import { SpinnerDlgComponent } from '../shared/spinner-dlg/spinner-dlg.component';
import { JwtHelper } from '../shared/classes/jwt-helper';
import { AccessToken } from '../shared/classes/access-token';


@Injectable()
export class MaterialTrackResolverService implements Resolve<ProjectMaster[]> {

  constructor(private uiUtilService: UiUtilsService, private materialTrackService: MaterialTrackService, private router: Router) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<ProjectMaster[]> {
    // this.uiUtilService.openDialog(SpinnerDlgComponent, null);
    return this.materialTrackService.getListProjects();
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