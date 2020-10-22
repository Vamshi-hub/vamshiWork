import { Injectable } from '@angular/core';
import { MatSnackBar, MatDialog } from '@angular/material'
import { ActivatedRouteSnapshot } from '@angular/router';
import { Observable } from 'rxjs';

@Injectable()
export class UiUtilsService {
  isDialogOpen = false;

  constructor(public snackBar: MatSnackBar, public dialog: MatDialog) {

    this.dialog.afterOpen.subscribe(() => {
      this.isDialogOpen = true;
    })
    this.dialog.afterAllClosed.subscribe(() => {
      this.isDialogOpen = false;
    })
  }

  openSnackBar(message: string, action: string, duration: number = 3000) {
    let snackBarRef = this.snackBar.open(message, action, {
      duration: duration,
    });

    snackBarRef.onAction().subscribe(() => {
      snackBarRef.dismiss();
    });
  }

  openDialog(dialogRef: any, data: any, disableClose = false) {
    if (!this.isDialogOpen) {
      this.dialog.open(dialogRef, {
        data: data,
        disableClose: disableClose
      });
    }
  }

  closeAllDialog() {
    this.dialog.closeAll();
  }

  getFullPath(route: ActivatedRouteSnapshot): string {
    if (route.routeConfig) {
      if (route.parent)
        return this.getFullPath(route.parent) + "/" + route.routeConfig.path;
      else
        return route.routeConfig.path;
    }
    else
      return "";
  }

  loadScript(scriptUrl: string) {
    return new Promise((resolve, reject) => {
      const scriptElement = document.createElement('script');
      scriptElement.src = scriptUrl;
      scriptElement.onload = resolve;
      scriptElement.onerror = reject;
      document.body.appendChild(scriptElement);
    });
  }

  loadScripts(scriptUrls: string[]) {
    return new Promise((resolve, reject) => {
      for(let scriptUrl of scriptUrls){
        const scriptElement = document.createElement('script');
        scriptElement.src = scriptUrl;
        scriptElement.onload = resolve;
        scriptElement.onerror = reject;
        document.body.appendChild(scriptElement);
      }
    });
  }
}
