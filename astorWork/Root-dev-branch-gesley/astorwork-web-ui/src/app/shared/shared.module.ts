import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { UiUtilsService } from './ui-utils.service'
import { AuthenticationService } from './authentication.service';
import { MatDialogModule, MatProgressSpinnerModule, MatDividerModule, MatButtonModule, MatCardModule } from '@angular/material';
import { SpinnerDlgComponent } from './spinner-dlg/spinner-dlg.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { VideoDlgComponent } from './video-dlg/video-dlg.component';
import { ChangePwDlgComponent } from '../user-account/change-password/result-dlg/result-dlg.component';
import { ResultDlgComponent } from '../material-track/create-mrf/result-dlg/result-dlg.component';
import { CommonLoadingComponent } from './common-loading/common-loading.component';
import { SlideShowComponent } from '../material-track/list-defect/slide-show/slide-show.component';
import { JobSlideShowComponent } from '../job-track/job-qc/slide-show/slide-show.component';
import { JobTasksSlideShowComponent } from '../job-track/job-tasks/slide-show/slide-show.component';
import { SampleViewComponent } from '../material-track/import-material/sample-view/sample-view.component';
import { GlobalErrorComponent } from './global-error/global-error.component';
import { SharedRoutingModule } from './shared-routing.module';
import { ShowQrcodeComponent } from '../material-track/list-material/show-qrcode/show-qrcode.component';

@NgModule({
  imports: [
    CommonModule,
    FlexLayoutModule,
    MatSnackBarModule,
    MatDialogModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatButtonModule,
    MatCardModule,
    SharedRoutingModule
  ],
  declarations: [SpinnerDlgComponent, VideoDlgComponent, CommonLoadingComponent, GlobalErrorComponent],
  providers: [
    MatSnackBarModule,
    MatDialogModule,
    UiUtilsService,
    AuthenticationService
  ],
  entryComponents: [
    ChangePwDlgComponent,
    SpinnerDlgComponent,
    VideoDlgComponent,
    ResultDlgComponent,
    SlideShowComponent,
    JobSlideShowComponent,
    JobTasksSlideShowComponent,
    SampleViewComponent,
    ShowQrcodeComponent
  ],
  exports: [CommonLoadingComponent]
})
export class SharedModule { }
