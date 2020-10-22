
import {map,  tap, catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable, throwError, of } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams } from '@angular/common/http';

import { environment } from '../../environments/environment';
import { MaterialMaster } from './classes/material-master'
import { TradeAssociation } from './classes/trade-association';
import { MaterialTypeMaster } from './classes/materialType-master';
import { MrfLocation } from './classes/mrf-location';
import { Role } from '../shared/classes/role';

import { Page } from '../shared/classes/page';
import { Module } from '../shared/classes/module';
import { RoleDetails } from '../shared/classes/role-details';
import { QCDefect } from './classes/qc-defect';
import { QCPhoto } from './classes/qc-photo';
import { QCCase } from './classes/qc-case';
import { ForgeToken, PowerBIAuthResult, PowerBIEmbedToken } from '../shared/classes/access-token';

import * as moment from 'moment';
import { ChecklistItemMaster } from './classes/checklistItem-master';
import { JobSchedule } from './classes/job-schedule';
import { TradeMaster } from './classes/trade-master';
import { JobStatus } from './classes/job-status';
import { DailyStatus } from './classes/dailyStatus';
import { Jobstarted } from './classes/jobstarted';

@Injectable()
export class JobTrackService {
  url_base = '/job-tracking';

  common_http_options = {
    headers: new HttpHeaders({
      'Content-Type': 'application/json'
    })
  };

  constructor(private http: HttpClient) {
    const auth_token = window.sessionStorage.getItem('auth_token');
    if (auth_token != null) {
      this.common_http_options.headers = new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${auth_token}`
      });
    }
  }

  private handleError(error: HttpErrorResponse) {
    console.error(error);
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an ErrorObservable with a user-facing error message
    if (error.statusText)
      return throwError(error.statusText);
    else
      return throwError(
        'Something bad happened; please try again later.');
  };

  getTradeAssociations(projectID: number): Observable<TradeAssociation[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/trade-associations`;
    return this.http.get<TradeAssociation[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getMaterialTypes(projectID: number): Observable<MaterialTypeMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/materialTypes`;
    return this.http.get<MaterialTypeMaster[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getChecklistItems(projectID: number): Observable<ChecklistItemMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/checklist-items`;
    return this.http.get<MaterialTypeMaster[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateTradeAssociation(projectID: number, tradeAssociations: TradeAssociation[]) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/trade-associations`;
    return this.http.put(endpoint, tradeAssociations).pipe(
      catchError(this.handleError)
    );
  }

  getLocationForJobSchedule(projectId: number, blk: string): Observable<MrfLocation[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-schedule/location?block=${blk}`;
    return this.http.get<MrfLocation[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getJobs(projectID: number, blk: string, lvl: string, materialType: string, subcon: string, lastMaterialIndex : number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/job-schedule?block=${blk}&level=${lvl}&materialType=${materialType}&subcon=${subcon}&lastMaterialIndex=${lastMaterialIndex}`;
    return this.http.get<JobSchedule[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }
  //service for  load on demand

  getScheduleJobs(projectID: number,lastMaterialIndex:number,pageSize:number,blk: string, lvl: string, materialType: string, subcon: string) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/job-schedule/jobschedulelist?lastMaterialIndex=${lastMaterialIndex}&pageSize=${pageSize}&block=${blk}&level=${lvl}&materialType=${materialType}&subcon=${subcon}`;
    return this.http.get<JobSchedule[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  saveJobs(projectID: number, lstJobScheduleDetails: JobSchedule[]) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/job-schedule`;
    return this.http.post(endpoint, lstJobScheduleDetails)
      .pipe(
        catchError(this.handleError)
      );
  }

  getMaterials(projectID: number, blk: string): Observable<MaterialMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/materials?block=${blk}`;
    return this.http.get<MaterialMaster[]>(endpoint, this.common_http_options)
      .pipe(
        tap(x => x.forEach((material) => {
          if (material.stageName == null) {
            if (material.mrfNo == null)
              material.stageName = "Unplanned";
            else
              material.stageName = "Planned";
          }
        })),
        catchError(this.handleError)
      );
  }

  getTrades(projectID:number):Observable<TradeMaster[]>{
    const endpoint=`${environment.api_base_url}${this.url_base}/projects/${projectID}/trades`;
    return this.http.get<TradeMaster[]>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );
  }

  //Import Checklist Types
  importChecklist(formData: FormData, projectID: number,tradeID:number,checklistType:string,materialStageID:number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectID}/checklist-items/import-checklist?trade_id=${tradeID}&checklist_type=${checklistType}&material_stage_id=${materialStageID}`;
    return this.http.post(endpoint, formData)
      .pipe(
        catchError(this.handleError)
      );
  }
  
  importJob(formData: FormData, project_Id: number) {

    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${project_Id}/trades/import-jobs`;
    return this.http.post(endpoint, formData)
      .pipe(
        catchError(this.handleError)
      );
  }
  
  importMaterialTypes(formData: FormData, project_Id: number) {

    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${project_Id}/materialtypes/Import-MaterialTypes`;
    return this.http.post(endpoint, formData)    .
      pipe(
        catchError(this.handleError)

      );
  }
             //**************/ Import PPVC schedule \*************\
  importJobSchedule(formData: FormData, project_Id: number) {
 console.log('Service Calling');
 console.log(formData);
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${project_Id}/job-schedule/import-JobSchedule`;
    return this.http.post(endpoint, formData)    .
      pipe(
        catchError(this.handleError)

      );
  }

  getPrjectProgress(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/progress`;
    return this.http.get(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getDelayedMaterials(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/delayed-materials`;
    return this.http.get(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getQcOpenMaterialsByProjectId(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/qc-open-and-daily-status`;
    return this.http.get(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }
  //#endregion

  getRoles(): Observable<Role[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles`;
    return this.http.get<Role[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  //#region Role Methods
  getDefaultPages(): Observable<Page[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles/default-pages`;
    return this.http.get<Page[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getModules(): Observable<Module[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles/pages`;
    return this.http.get<Module[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getRoleDetails(roleId: number): Observable<RoleDetails> {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles/${roleId}`;
    return this.http.get<RoleDetails>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }
  createRole(roleDetails: RoleDetails) {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles`;
    return this.http.post(endpoint, roleDetails).pipe(
      catchError(this.handleError)
    );
  }

  //#endregion

  getQCDefects(case_id: number,projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/defect?case_id=${case_id}`;

    const params = new HttpParams().set("case_id", String(case_id));
    return this.http.get<JobStatus[]>(endpoint, { params: params })
      .pipe(
        catchError(this.handleError)
      );
  }

  getQCPhotos(projectId: number,defect_id:number,jobscheduleID:number,MaterialStageAuditID:number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/photo?defect_id=${defect_id}&jobscheduleID=${jobscheduleID}&MaterialStageAuditID=${MaterialStageAuditID}`;
    return this.http.get<QCPhoto[]>(endpoint)
    .pipe(
      catchError(this.handleError)
    );
  }
  getdefects(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/defect`;
  return this.http.get<Jobstarted[]>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );
}
    getarchiqcjobschecklists(projectId: number) {
      const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/archiqc-checklists-jobs`;
    return this.http.get<Jobstarted[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }
  getStructuraljobs(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/struct-checklist`;
  return this.http.get<Jobstarted[]>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );
}

  uploadMaterialFile(materialTemplate: FormData, project_Id: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${project_Id}/materials/import-template`;
    return this.http.post(endpoint, materialTemplate)
      .pipe(
        catchError(this.handleError)
      );
  }

  getForgeToken() {
    const endpoint = `${environment.api_base_url}${this.url_base}/forge-auth`;
    return this.http.get<ForgeToken>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getPowerBIToken() {
    var tokenStr = '';
    var tokenExpireStr = localStorage.getItem("powerbi_token_expire_time");
    if (tokenExpireStr) {
      var tokenExpireTime = moment(tokenExpireStr);
      var now = moment();
      if (tokenExpireTime > now.add(5, 'minutes')) {
        tokenStr = localStorage.getItem('powerbi_token');
      }
      else {
        console.log('Power BI token expired');
      }
    }

    if (tokenStr)
      return of(tokenStr);
    else {
      const endpoint = `${environment.api_base_url}/powerbi/authenticate`;
      return this.http.get<PowerBIAuthResult>(endpoint).pipe(
        map((authResult) => {
          localStorage.setItem('powerbi_token', authResult.access_token);
          localStorage.setItem("forge_token_expire_time",
            moment().add(authResult.expires_in, "seconds").format());

          return authResult.access_token;
        }))
        .pipe(
          catchError(this.handleError)
        );
    }
  }

  getPowerBIEmbedToken(powerbi_token: string, report_guid: string) {
    const endpoint = `${environment.api_base_url}/powerbi/embed-token`;
    const params = new HttpParams().set("powerbi_token", powerbi_token).set("report_guid", report_guid);
    return this.http.get<PowerBIEmbedToken>(endpoint, { params: params })
      .pipe(
        catchError(this.handleError)
      );
  }

getJobStatus(projectId: number): Observable<JobStatus> {
  const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/job-status`;
  return this.http.get<JobStatus>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );
}
getJobList(projectId: number): Observable<JobStatus> {
  const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/job-statuslist`;
  return this.http.get<JobStatus>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );
}
getDailyJobProgress(projectId: number): Observable<DailyStatus> {
  const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/daily-job-status`;
  return this.http.get<DailyStatus>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );

}
getOverallJobProgress(projectId:number){
  const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/job-dashboard/overall-job-progress`;
  return this.http.get<DailyStatus>(endpoint, this.common_http_options)
    .pipe(
      catchError(this.handleError)
    );

}
}