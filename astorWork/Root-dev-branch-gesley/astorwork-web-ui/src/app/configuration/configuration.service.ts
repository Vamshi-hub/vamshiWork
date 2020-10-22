import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpErrorResponse } from '@angular/common/http';
import { tap, catchError } from 'rxjs/operators';
import { Observable, throwError } from 'rxjs';
import { OrganisationMaster } from '../material-track/classes/organisation-master';
import { environment } from '../../environments/environment';
import { LocationMaster } from '../material-track/classes/location-master';
import { StageMaster } from '../material-track/classes/stage-master';
import { QRCode } from '../material-track/classes/qrCode';
import { SiteMaster } from './classes/site-master';
import { errorHandler } from '@angular/platform-browser/src/browser';
import { MAT_MENU_DEFAULT_OPTIONS_FACTORY } from '@angular/material/menu/typings/menu-directive';
import { Country } from './classes/country';
import { NotificationTimer } from './classes/notification-timer';


@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {
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
    if (error.statusText)
      return throwError(error.statusText);
    else
      return throwError(
        'Something bad happened; please try again later.');
  }

  getOrganisations(): Observable<OrganisationMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/organisations`;

    return this.http.get<OrganisationMaster[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  createOrganisation(organisation: OrganisationMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/organisations`;
    return this.http.post(endpoint, organisation).pipe(
      catchError(this.handleError)
    );
  }

  updateOrganisation(organisationID: number, organisation: OrganisationMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/organisations/${organisationID}`;
    return this.http.put(endpoint, organisation).pipe(
      catchError(this.handleError)
    );
  }

  getOrganisationDetails(organisationID: number): Observable<OrganisationMaster> {
    const endpoint = `${environment.api_base_url}${this.url_base}/organisations/${organisationID}`;
    return this.http.get<OrganisationMaster>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getLocations(): Observable<LocationMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/locations`;
    return this.http.get<LocationMaster[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getLocationDetais(locationId: number): Observable<LocationMaster> {
    const endpoint = `${environment.api_base_url}${this.url_base}/locations/${locationId}`;
    return this.http.get<LocationMaster>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  editLocation(location: LocationMaster): Observable<Object> {
    const endpoint = `${environment.api_base_url}${this.url_base}/locations/${location.id}`;
    return this.http.put(endpoint, location)
      .pipe(
        catchError(this.handleError)
      );
  }

  createLocation(location: LocationMaster): Observable<Object> {
    const endpoint = `${environment.api_base_url}${this.url_base}/locations`;
    return this.http.post(endpoint, location)
      .pipe(
        catchError(this.handleError)
      );
  }

  //#region stage master methods

  getMaterialStages(): Observable<StageMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/material-stages`;
    return this.http.get<StageMaster[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }
  createStage(stage: any) {
    console.log(stage);
    const endpoint = `${environment.api_base_url}${this.url_base}/material-stages`;
    return this.http.post(endpoint, stage).pipe(
      catchError(this.handleError)
    );
  }
  updateStage(stageID: number, stage: StageMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/material-stages/${stageID}`;
    return this.http.put(endpoint, stage).pipe(
      catchError(this.handleError)
    );
  }
  updateSorting(lstStages: StageMaster[]) {
    const endpoint = `${environment.api_base_url}${this.url_base}/material-stages/stage-list`;
    return this.http.post<StageMaster[]>(endpoint, lstStages).pipe(
      catchError(this.handleError)
    );
  }

  getMaterialtypes(): Observable<string[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/projects/-1/material-types`;
    return this.http.get<string[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }
  //#endregion


  //#region  generate/Import methods

  getGeneratedQRCodes(quantity: Number, label: string): Observable<QRCode[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/trackers/generate-qr-codes?qty=${quantity}&label=${label}`;
    return this.http.get<QRCode[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  uploadFile(formData: FormData, tagType: string) {
    const endpoint = `${environment.api_base_url}${this.url_base}/trackers/import-trackers?type=${tagType}`;
    return this.http.post(endpoint, formData)
      .pipe(
        catchError(this.handleError)
      );
  }

  //#endregion

  //#region Site master methods...

  getSites(): Observable<SiteMaster[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/sites`;
    return this.http.get<SiteMaster[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getCountries(): Observable<Country[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/sites/countries`;
    return this.http.get<Country[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  getSiteDetails(id: number): Observable<SiteMaster> {
    const endpoint = `${environment.api_base_url}${this.url_base}/sites/${id}`;
    return this.http.get<SiteMaster>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  createSite(site: SiteMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/sites`;
    return this.http.post(endpoint, site)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateSite(id: number, site: SiteMaster) {
    const endpoint = `${environment.api_base_url}${this.url_base}/sites/${id}`;
    return this.http.put(endpoint, site)
      .pipe(
        catchError(this.handleError)
      );
  }

  //#endregion

  //#region Notification

  getNotifications(id: number, type: string): Observable<NotificationTimer[]> {
    const endpoint = `${environment.api_base_url}${this.url_base}/notification-timer/${id}?type=${type}`;
    return this.http.get<NotificationTimer[]>(endpoint)
      .pipe(
        catchError(this.handleError)
      );
  }

  postNotification(notifications: NotificationTimer[]) {
    const endpoint = `${environment.api_base_url}${this.url_base}/notification-timer`;
    return this.http.post(endpoint, notifications)
      .pipe(
        catchError(this.handleError)
      );
  }
  //#endregion
}
