import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ListMaterialComponent } from './list-material/list-material.component';
import { MaterialTrackService } from './material-track.service';
import { MaterialDetailsComponent } from './material-details/material-details.component';

import { MaterialTrackRoutingModule } from './material-track-routing.module';
import { CreateMrfComponent } from './create-mrf/create-mrf.component';
import { ResultDlgComponent } from './create-mrf/result-dlg/result-dlg.component';
import { ListMrfComponent } from './list-mrf/list-mrf.component';
import { ListBimSyncComponent } from './list-bim-sync/list-bim-sync.component';
import { BimSyncDetailsComponent } from './bim-sync-details/bim-sync-details.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { DashboardConsultantComponent } from './dashboard-consultant/dashboard-consultant.component';
import { DashboardMainconComponent } from './dashboard-maincon/dashboard-maincon.component';
import { DashboardAlecComponent } from './dashboard-alec/dashboard-alec.component';
import { MccColorPickerModule } from 'material-community-components';
import { MatIconModule, MatButtonModule, MatTableModule, MatDialogModule, MatPaginatorModule, MatSortModule, MatFormFieldModule, MatInputModule, MatGridListModule, MatSelectModule, MatChipsModule, MatDividerModule, MatStepperModule, MatListModule, MatExpansionModule, MatCardModule, MatSidenavModule, MatProgressBarModule, MatProgressSpinnerModule, MatDatepickerModule, MatTabsModule, MatMenuModule, MatToolbarModule, MatCheckboxModule, MatRadioModule, MatSlideToggle, MatSlideToggleModule, MatAutocompleteModule, MatTooltipModule } from '@angular/material';

import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatMomentDateModule } from '@angular/material-moment-adapter';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '../shared/shared.module';
import { SlideShowComponent } from './list-defect/slide-show/slide-show.component';
import { UICarouselModule } from "ui-carousel";
import { QcDetailsComponent } from './qc-details/qc-details.component';
import { ListDefectComponent } from './list-defect/list-defect.component';
import { ImportMaterialComponent } from './import-material/import-material.component';
import { ImportFileComponent } from './import-file/import-file.component';
import { SampleViewComponent } from './import-material/sample-view/sample-view.component';
import { SampleViewFileComponent } from './import-file/sample-view/sample-view.component';
import { ListReportsComponent } from './list-reports/list-reports.component';
import { PowerbiViewerComponent } from './powerbi-viewer/powerbi-viewer.component';
import { ShowQrcodeComponent } from './list-material/show-qrcode/show-qrcode.component';
import { NgxQRCodeModule } from 'ngx-qrcode2';
import { MaterialQcComponent } from './material-qc/material-qc.component';
import { MaterialSlideShowComponent } from './material-qc/slide-show/slide-show.component';
import { MaterialDashboardComponent } from './material-dashboard/material-dashboard.component';
import { MaterialstagedashboardComponent } from './materialstagedashboard/materialstagedashboard.component';

@NgModule({
  imports: [
    CommonModule,
    MaterialTrackRoutingModule,
    SharedModule,
    FlexLayoutModule,
    FormsModule, 
    ReactiveFormsModule,
    MatMomentDateModule,
    MatDialogModule,
    MatButtonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatFormFieldModule,
    MatInputModule,
    MatGridListModule,
    MatSelectModule,
    MatChipsModule,
    MatDividerModule,
    MatStepperModule,
    MatListModule,
    MatIconModule,
    MatExpansionModule,
    MatCardModule,
    MatSidenavModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatDatepickerModule,
    MatTabsModule,
    MatMenuModule,
    MatToolbarModule,
    MatCheckboxModule,
    MatRadioModule,
    UICarouselModule,
    MatAutocompleteModule,
    MatTooltipModule,
    NgxQRCodeModule,
    MccColorPickerModule.forRoot({
      empty_color: 'transparent'
    }),
    BrowserAnimationsModule,
    MatSlideToggleModule
  ],
  declarations: [ListMaterialComponent, MaterialDetailsComponent, CreateMrfComponent, ResultDlgComponent, ListMrfComponent, DashboardComponent, DashboardConsultantComponent, DashboardMainconComponent, DashboardAlecComponent, ListBimSyncComponent, BimSyncDetailsComponent, SlideShowComponent, QcDetailsComponent,ListDefectComponent, ImportMaterialComponent, ImportFileComponent, SampleViewComponent, SampleViewFileComponent, ListReportsComponent, PowerbiViewerComponent, ShowQrcodeComponent, MaterialDashboardComponent,MaterialQcComponent,MaterialSlideShowComponent, MaterialstagedashboardComponent],
  providers: [MaterialTrackService],
  entryComponents: [MaterialSlideShowComponent],
})
export class MaterialTrackModule { }
