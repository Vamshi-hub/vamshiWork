import { throwError, Observable } from 'rxjs';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpResponse,
  HttpErrorResponse,
  HttpHandler,
  HttpEvent
} from '@angular/common/http';

@Injectable()
export class MyHttpLogInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let authorization = '';
    const accessTokenType = window.localStorage.getItem("access_token_type");
    const accessToken = window.localStorage.getItem("access_token");
    if (accessTokenType && accessToken) {
      authorization = `${accessTokenType} ${accessToken}`;
    }

    const customReq = request.clone({
      headers: request.headers.set('Authorization', authorization)
    });

    return next
      .handle(customReq)
      .map(ev => {
        if (ev instanceof HttpResponse) {
          if (ev.body.status === 0)
            ev = ev.clone({ body: ev.body.data });
          else
            return ev.clone({ status: ev.body.status, statusText: ev.body.message });
        }
        return ev;
      })
      .catch(response => {
        if (response instanceof HttpErrorResponse) {
          console.log('Processing http error', response);
        }
        return throwError(response);
      });
  }
}
