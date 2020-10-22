import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ConfigurationRoutingModule } from './configuration-routing.module';
import { UserMasterComponent } from './user-master/user-master.component';
import { UserDetailsComponent } from './user-details/user-details.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { MatTableModule, MatPaginatorModule, MatButtonModule, MatIconModule, MatFormFieldModule, MatSelectModule, MatCheckboxModule, MatCardModule, MatInputModule, MatRadioGroup, MatRadioModule, MatDialogModule, MatSortModule, MatGridListModule, MatChipsModule, MatDividerModule, MatStepperModule, MatListModule, MatExpansionModule, MatSidenavModule, MatProgressBarModule, MatProgressSpinnerModule, MatDatepickerModule, MatTabsModule, MatMenuModule, MatToolbarModule, MatTooltipModule, MatSlideToggleModule } from '@angular/material';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RoleMasterComponent } from './role-master/role-master.component';
import { RoleDetailsComponent } from './role-details/role-details.component';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { VendorMasterComponent } from './vendor-master/vendor-master.component';
import { VendorDetailsComponent } from './vendor-details/vendor-details.component';
import { LocationMasterComponent } from './location-master/location-master.component';
import { LocationDetailsComponent } from './location-details/location-details.component';
import { GenerateQrCodeComponent } from './generate-qr-code/generate-qr-code.component';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { ListProjectComponent } from './list-project/list-project.component';
import { ProjectDetailsComponent } from './project-details/project-details.component';
import { MccColorPickerModule } from 'material-community-components';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SortableModule } from '@progress/kendo-angular-sortable';
import { StageMasterComponent } from './stage-master/stage-master.component';
import { DetailStageComponent } from './detail-stage/detail-stage.component';
import { SharedModule } from '../shared/shared.module';
import { SiteMasterComponent } from './site-master/site-master.component';
import { SiteDetailsComponent } from './site-details/site-details.component';
import { NotificationConfigComponent } from './notification-config/notification-config.component';


import { DateInputsModule } from '@progress/kendo-angular-dateinputs';
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
    DateInputsModule,
    SharedModule,
    MccColorPickerModule.forRoot({
      empty_color: 'transparent'
    }),
    BrowserAnimationsModule, SortableModule,MatSlideToggleModule
  ],
  declarations: [UserMasterComponent, UserDetailsComponent, RoleMasterComponent, RoleDetailsComponent, VendorMasterComponent, VendorDetailsComponent, LocationMasterComponent, LocationDetailsComponent, GenerateQrCodeComponent, ListProjectComponent, ProjectDetailsComponent,StageMasterComponent,DetailStageComponent, SiteMasterComponent, SiteDetailsComponent, NotificationConfigComponent]
})
export class ConfigurationModule { }
