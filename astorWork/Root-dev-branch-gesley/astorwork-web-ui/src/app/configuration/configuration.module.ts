import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ConfigurationRoutingModule } from './configuration-routing.module';
import { UserMasterComponent } from './user-master/user-master.component';
import { UserDetailsComponent } from './user-details/user-details.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatSelectModule, MatCheckboxModule, MatCardModule, MatInputModule, MatRadioGroup, MatRadioModule, MatDialogModule, MatSortModule, MatGridListModule, MatChipsModule, MatDividerModule, MatStepperModule, MatListModule, MatExpansionModule, MatSidenavModule, MatProgressBarModule, MatProgressSpinnerModule, MatDatepickerModule, MatTabsModule, MatMenuModule, MatToolbarModule, MatTooltipModule, MatSlideToggleModule, MatAutocompleteModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RoleMasterComponent } from './role-master/role-master.component';
import { RoleDetailsComponent } from './role-details/role-details.component';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { OrganisationMasterComponent } from './organisation-master/organisation-master.component';
import { OrganisationDetailsComponent } from './organisation-details/organisation-details.component';
import { LocationMasterComponent } from './location-master/location-master.component';
import { LocationDetailsComponent } from './location-details/location-details.component';
import { GenerateQrCodeComponent } from './generate-qr-code/generate-qr-code.component';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { ListProjectComponent } from './list-project/list-project.component';
import { ProjectDetailsComponent } from './project-details/project-details.component';
import { MccColorPickerModule } from 'material-community-components';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { StageMasterComponent } from './stage-master/stage-master.component';
import { DetailStageComponent } from './detail-stage/detail-stage.component';
import { SharedModule } from '../shared/shared.module';
import { SiteMasterComponent } from './site-master/site-master.component';
import { SiteDetailsComponent } from './site-details/site-details.component';
import { NotificationConfigComponent } from './notification-config/notification-config.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { NgxMaterialTimepickerModule } from 'ngx-material-timepicker';

@NgModule({
  imports: [
    CommonModule,
    ConfigurationRoutingModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    MatSelectModule,
    MatCheckboxModule,
    MatFormFieldModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    MatInputModule,
    MatRadioModule,
    MatMomentDateModule,
    MatDialogModule,
    MatSortModule,
    MatGridListModule,
    MatChipsModule,
    MatDividerModule,
    MatStepperModule,
    MatListModule,
    MatExpansionModule,
    MatSidenavModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatDatepickerModule,
    MatTabsModule,
    MatMenuModule,
    MatToolbarModule,
    NgxQRCodeModule,
    MatTooltipModule,
    MatSlideToggleModule,
    DragDropModule,
    SharedModule,
    MccColorPickerModule.forRoot({
      empty_color: 'transparent'
    }),
    NgxMaterialTimepickerModule.forRoot(),
    BrowserAnimationsModule,
    MatSlideToggleModule,
    MatAutocompleteModule
  ],
  declarations: [UserMasterComponent, UserDetailsComponent, RoleMasterComponent, RoleDetailsComponent, OrganisationMasterComponent, OrganisationDetailsComponent, LocationMasterComponent, LocationDetailsComponent, GenerateQrCodeComponent, ListProjectComponent, ProjectDetailsComponent, StageMasterComponent, DetailStageComponent, SiteMasterComponent, SiteDetailsComponent, NotificationConfigComponent]
})
export class ConfigurationModule { }
