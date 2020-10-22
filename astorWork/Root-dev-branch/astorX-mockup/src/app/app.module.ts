import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconModule, MatButtonModule, MatTableModule, MatDialogModule, MatPaginatorModule, MatSortModule, MatFormFieldModule, MatInputModule, MatGridListModule, MatSelectModule, MatChipsModule, MatDividerModule, MatStepperModule, MatListModule, MatExpansionModule, MatCardModule, MatSidenavModule, MatProgressBarModule, MatProgressSpinnerModule, MatDatepickerModule, MatTabsModule, MatMenuModule, MatToolbarModule, MatCheckboxModule, MatRadioModule, MatSlideToggle, MatSlideToggleModule, MatAutocompleteModule, MatTooltipModule } from '@angular/material';

import { FlexLayoutModule } from '@angular/flex-layout';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    FlexLayoutModule,
    MatCardModule,
    MatDividerModule,
    MatIconModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
