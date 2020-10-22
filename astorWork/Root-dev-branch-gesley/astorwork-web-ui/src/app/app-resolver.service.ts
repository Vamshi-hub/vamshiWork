import { Injectable } from '@angular/core';
import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { AccessToken } from './shared/classes/access-token';
import { JwtHelper } from './shared/classes/jwt-helper';
import { UiUtilsService } from './shared/ui-utils.service';
import { Observable ,  of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})

export class AccessRightResolver implements Resolve<number>{

  constructor(private uiUtilService: UiUtilsService) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<number> {
    let url: string = this.uiUtilService.getFullPath(route);
    let right = 0;
    const accessTokenStr = window.localStorage.getItem('access_token');
    if (accessTokenStr) {
      const accessToken = JwtHelper.decodeToken(accessTokenStr) as AccessToken;
      for (let ar of accessToken.role.pageAccessRights) {
        if (ar.url == url) {
          right = ar.right;
          break;
        }
      }
    }

    return of(right);
  }
}