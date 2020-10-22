import { BrowserModule } from '@angular/platform-browser';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { SharedModule } from './shared/shared.module'
import { PeopleTrackModule } from './people-track/people-track.module';
import { MyHttpLogInterceptor } from './app.interceptor';
import { ConfigurationModule } from './configuration/configuration.module';
import { UserAccountModule } from './user-account/user-account.module';
import { MaterialTrackModule } from './material-track/material-track.module';
import { TopMenuComponent } from './top-menu/top-menu.component';
import { MatIconModule, MatMenuModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSlideToggle, MatSlideToggleModule } from '@angular/material';
import { LoginComponent } from './login/login.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { ReactiveFormsModule, FormsModule, ValidatorFn, AbstractControl, ValidationErrors } from '@angular/forms';
import { ForgetPasswordComponent } from './forget-password/forget-password.component';
import { ForgeViewerComponent } from './forge-viewer/forge-viewer.component';

@NgModule({
  declarations: [
    AppComponent,
    TopMenuComponent,
    LoginComponent,
    ForgetPasswordComponent,
    ForgeViewerComponent
  ],
  imports: [
    // Angular modules
    BrowserModule,
    HttpClientModule,
    BrowserAnimationsModule,
    FormsModule,
    // astorWork modules
    AppRoutingModule,
    SharedModule,
    MaterialTrackModule,
    PeopleTrackModule,
    ConfigurationModule,
    UserAccountModule,
    // Third-party modules
    FlexLayoutModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatSlideToggleModule
  ],
  providers: [HttpClientModule, SharedModule
    , { provide: HTTP_INTERCEPTORS, useClass: MyHttpLogInterceptor, multi: true }],
  bootstrap: [AppComponent]
})
export class AppModule { }