import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FlatpickrModule } from 'angularx-flatpickr';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { NgbModalModule } from '@ng-bootstrap/ng-bootstrap';
import { JobTrackingRoutingModule } from './job-track-routing.module';
import { TradeAssociationComponent } from './trade-association/trade-association.component';
import { JobSchedulingComponent } from './job-scheduling/job-scheduling.component';
import { JobTasksComponent } from './job-tasks/job-tasks.component';
import { JobQCComponent } from './job-qc/job-qc.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule, MatMenuModule, MatButtonModule, MatCardModule, MatFormFieldModule, MatInputModule, MatSlideToggle, MatSlideToggleModule, MatProgressBarModule, MatProgressBar, MatSortModule, MatListModule} from '@angular/material';
import {MatSelectModule} from '@angular/material/select';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatPaginatorModule} from '@angular/material/paginator';
import { FlexLayoutModule } from '@angular/flex-layout';
import { UICarouselModule } from "ui-carousel";
import { JobSlideShowComponent } from './job-qc/slide-show/slide-show.component';
import { JobTasksSlideShowComponent } from './job-tasks/slide-show/slide-show.component';
import {MatDialogModule} from '@angular/material/dialog';
import {MatAutocompleteModule} from '@angular/material/autocomplete';
import { ImportchecklistComponent } from './import-checklist/import-checklist.component';
import { JobTrackService } from './job-track.service';
import { SharedModule } from '../shared/shared.module';
import { ImportJobscheduleComponent } from './job-scheduling/import-jobschedule/import-jobschedule.component';
import { JobDashboardComponent } from './job-dashboard/job-dashboard.component';
import { SampleViewComponent } from './import-checklist/sample-view/sample-view.component';
import { SlideShowComponent } from '../material-track/list-defect/slide-show/slide-show.component';
import { JobscheduleSampleViewComponent } from './jobschedule-sample-view/jobschedule-sample-view.component';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
@NgModule({
  imports: [
    BrowserAnimationsModule, 
    CommonModule,
    FormsModule,
    NgbModalModule,
    FlatpickrModule.forRoot(),
    CalendarModule.forRoot({
      provide: DateAdapter,
      useFactory: adapterFactory
    }),
    JobTrackingRoutingModule, 
    MatTableModule, 
    MatSortModule,
    MatFormFieldModule,
    MatSelectModule,
    MatDatepickerModule,
    MatIconModule,
    MatProgressBarModule,
    MatPaginatorModule,
    MatMenuModule,
    MatInputModule,
    MatButtonModule,
    FlexLayoutModule,
    UICarouselModule,
    MatDialogModule,
    MatAutocompleteModule,
    MatCardModule,
    ReactiveFormsModule,
    SharedModule,
    MatListModule,
    InfiniteScrollModule,
  ],
  declarations: [TradeAssociationComponent, JobSchedulingComponent, JobQCComponent, JobTasksComponent, JobSlideShowComponent, JobTasksSlideShowComponent,ImportchecklistComponent, ImportJobscheduleComponent, JobDashboardComponent, SampleViewComponent, JobscheduleSampleViewComponent],
  providers: [JobTrackService],
  entryComponents: [ SampleViewComponent,JobscheduleSampleViewComponent]
})
export class JobTrackModule {}