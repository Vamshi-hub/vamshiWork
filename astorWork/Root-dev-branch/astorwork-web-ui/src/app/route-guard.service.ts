import { Injectable } from '@angular/core';
import { CanActivate, CanActivateChild, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { JwtHelper } from './shared/classes/jwt-helper';
import { AccessToken } from './shared/classes/access-token';
import { UiUtilsService } from './shared/ui-utils.service';

@Injectable({
  providedIn: 'root'
})
export class RouteGuardService implements CanActivate, CanActivateChild {

  constructor(private uiUtilService: UiUtilsService, private router: Router) { }

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    return true;
  }

  canActivateChild(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean {
    let url: string = this.uiUtilService.getFullPath(route);
    let canActivate: boolean = false;

    var accessTokenStr = window.localStorage.getItem("access_token");
    if (accessTokenStr) {
      const decodedJson = JwtHelper.decodeToken(accessTokenStr);
      const accessToken = decodedJson as AccessToken;
      
      for (let ar of accessToken.role.pageAccessRights) {   
        if (url == ar.url && ar.right > 0) {
          canActivate = true;
          break;
        }
      }

      

      if (!canActivate) {
        //this.uiUtilService.openSnackBar("You don't have access to the page", "OK");
        let message = `You don't have access to the page ${state.url}`;
        sessionStorage.setItem("message", message);
        this.router.navigate(['/global/error']);
      }

      return canActivate;
    }
  }
}