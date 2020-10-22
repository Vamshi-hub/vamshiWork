
import {map,  tap, catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { Observable, throwError, of } from 'rxjs';
import { HttpClient, HttpHeaders, HttpErrorResponse, HttpParams } from '@angular/common/http';

import { environment } from '../../environments/environment';
import { MaterialMaster } from './classes/material-master'
import { ProjectMaster } from './classes/project-master';
import { MaterialDetail } from './classes/material-detail';
import { OrganisationMaster } from './classes/organisation-master';
import { Mrf } from './classes/mrf-master';
import { MrfLocation } from './classes/mrf-location';
import { MrfOrganisation } from './classes/mrf-organisation';
import { BIMSyncSession } from './classes/bim-sync-session';

import { ProjectStats } from './classes/Project-Stats';
import { Users } from './classes/users';
import { UserDetails } from './classes/user-details';
import { Role } from '../shared/classes/role';
import { Password } from './classes/password';
import { Page } from '../shared/classes/page';
import { Module } from '../shared/classes/module';
import { RoleDetails } from '../shared/classes/role-details';
import { QCDefect } from './classes/qc-defect';
import { QCPhoto } from '../job-track/classes/qc-photo';
import { QCCase } from './classes/qc-case';
import { ForgeToken, PowerBIAuthResult, PowerBIEmbedToken } from '../shared/classes/access-token';
import { PowerBIReport } from './classes/power-bi-report';
import * as moment from 'moment';
import { BIMViewerProgress } from './classes/bim-viewer-progress';
import { PrintQRItem } from './classes/print-qr-item';
import { MaterialStageDetail } from './classes/material-stage-detail';
import { StageMaster } from './classes/stage-master';
//import { Moment } from 'moment';

@Injectable()
export class MaterialTrackService {
  url_base = '/material-tracking';

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

  getListProjects(): Observable<ProjectMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects`;
    return this.http.get<ProjectMaster[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getProjectInfo(id: number, filter: string): Observable<ProjectMaster> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${id}?filter=${filter}`;
    return this.http.get<ProjectMaster>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getMaterials(projectId: number, blk: string): Observable<MaterialMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/materials?block=${blk}`;
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

  getMaterialDetail(projectId, materialId: number): Observable<MaterialDetail> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/materials/${materialId}`;
    return this.http.get<MaterialDetail>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getListOrganisations(): Observable<OrganisationMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/organisations`;
    return this.http.get<OrganisationMaster[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getLocationForNewMRF(projectId: number, blk: string): Observable<MrfLocation[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/mrfs/location?block=${blk}`;
    return this.http.get<MrfLocation[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  getMaterialTypesForNewMRF(projectId: number, blk: string, lvl: string, zone: string): Observable<MrfOrganisation[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/mrfs/material?block=${blk}&level=${lvl}&zone=${zone}`;
    return this.http.get<MrfOrganisation[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }

  createMRF(projectId: number, mrf: Mrf): Observable<Object> {
    
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/mrfs`;
    console.log(mrf);
    return this.http.post(endpoint, mrf)
      .pipe(
        catchError(this.handleError)
      );
  }

  getListMRFs(projectId: number, blk: string): Observable<Mrf[]> {
    let endpoint = ''
    if (blk) {
      endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/mrfs?block=${blk}`;
    }
    else {
      endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/mrfs`;
    }
    return this.http.get<Mrf[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateMaterialDetails(projectId: number, materialDetail: MaterialDetail) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/materials/${materialDetail.id}`;
    return this.http.put(endpoint, materialDetail)
      .pipe(
        catchError(this.handleError)
      );
  }

  getListBIMSyncSessions(projectId: number): Observable<BIMSyncSession[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/bim-sync`;
    return this.http.get<BIMSyncSession[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getBIMSyncSessionDetails(projectId: number, bimSyncSessionId: number): Observable<BIMSyncSession> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/bim-sync/${bimSyncSessionId}`;
    return this.http.get<BIMSyncSession>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  //#region Dashboard methods
  getProjectStats(projectId: number): Observable<ProjectStats> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/stats`;
    return this.http.get<ProjectStats>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }
  getMaterialStagestatus(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/material-stage-stats`;
    return this.http.get<MaterialStageDetail[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }
getreadytoprojectstats(projectId: number){
  const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/Readytoprojectstatus`;
  return this.http.get(endpoint)
    .pipe(
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

  getReadyToDeliveredMaterialsByProjectId(projectId: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/dashboard/ready-to-delivered-materials`;
    return this.http.get(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }


  //#endregion

  //#region User methods
  getUsers(isOrganisation: number | null): Observable<Users[]> {
    const endpoint = `${environment.api_base_url}/user/users?isOrganisation=${isOrganisation}`;
    return this.http.get<Users[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }
  
  getUserDetails(userId: number): Observable<UserDetails> {
    const endpoint = `${environment.api_base_url}/user/${userId}`;
    return this.http.get<UserDetails>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getRoles(): Observable<Role[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles`;
    return this.http.get<Role[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  createUser(user: Users) {
    const endpoint = `${environment.api_base_url}/user/create`;
    return this.http.post(endpoint, user).pipe(
      catchError(this.handleError)
    );
  }

  updateUser(userId: number, user: Users) {
    const endpoint = `${environment.api_base_url}/user/${userId}`;
    return this.http.put(endpoint, user).pipe(
      catchError(this.handleError)
    );
  }

  updateUserPassword(userId: number, password: Password) {
    const endpoint = `${environment.api_base_url}/user/${userId}/change-password`;
    return this.http.put(endpoint, password);
  }
  //#endregion

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

  updateRole(roleId: number, roleDetails: RoleDetails) {
    const endpoint = `${environment.api_base_url}${this.url_base}/roles/${roleId}`;
    return this.http.put(endpoint, roleDetails).pipe(
      catchError(this.handleError)
    );
  }
  //#endregion

  //#region Project Methods

  createProject(project: ProjectMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects`;
    return this.http.post(endpoint, project)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateProject(id: number, project: ProjectMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${id}`;
    return this.http.put(endpoint, project)
      .pipe(
        catchError(this.handleError)
      );
  }

  //#endregion

  getQCDefects(case_id: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/qc/defect`;

    const params = new HttpParams().set("case_id", String(case_id));
    return this.http.get<QCDefect[]>(endpoint, { params: params })
      .pipe(
        catchError(this.handleError)
      );
  }

  getQCPhotos(defect_id: number) {
    const endpoint = `${environment.api_base_url}${this.url_base}/qc/photo`;

    const params = new HttpParams().set("defect_id", String(defect_id));
    return this.http.get<QCPhoto[]>(endpoint, { params: params })
      .pipe(
        catchError(this.handleError)
      );

  }

  getCases(projectId: number): Observable<QCCase[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/qc/case`;
    const params = new HttpParams().set("project_id", String(projectId));
    return this.http.get<QCCase[]>(endpoint, { params: params })
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

  getForgeModelProgress(modelURN: string) {
    const endpoint = `${environment.api_base_url}${this.url_base}/bim-viewer/current-progress`;
    const params = new HttpParams().set("model_urn", String(modelURN));
    return this.http.get<BIMViewerProgress[]>(endpoint, { params: params })
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

  getPowerBIReports(powerbi_token: string) {
    const endpoint = `${environment.api_base_url}/powerbi/reports`;
    const params = new HttpParams().set("powerbi_token", powerbi_token);
    return this.http.get<PowerBIReport[]>(endpoint, { params: params })
      .pipe(
        catchError(this.handleError)
      );
  }

  getPowerBIEmbedToken(powerbi_token: string, report_guid: string) {
    const endpoint = `${environment.api_base_url}/powerbi/embed-token`;
    const params = new HttpParams().set("powerbi_token", powerbi_token).set("report_guid", report_guid);
    return this.http.get<PowerBIEmbedToken>(endpoint, { params: params })
      .pipe(
        catchError(this.handleError)
      );
  }

  getListQRCodeByMRF(projectId: number, mrfId: number): Observable<PrintQRItem[]>  {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/mrfs/${mrfId}/list-qr-code`;
    return this.http.get<PrintQRItem[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getMaterialStageCounts(projectId: number): Observable<StageMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/${projectId}/materialStageAudits/counts`;
    return this.http.get<StageMaster[]>(endpoint, this.common_http_options)
      .pipe(
        catchError(this.handleError)
      );
  }
}