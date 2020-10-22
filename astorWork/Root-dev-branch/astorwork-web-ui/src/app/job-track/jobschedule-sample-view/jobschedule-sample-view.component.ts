import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-jobschedule-sample-view',
  templateUrl: './jobschedule-sample-view.component.html',
  styleUrls: ['./jobschedule-sample-view.component.css']
})
export class JobscheduleSampleViewComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<JobscheduleSampleViewComponent>) { }

  ngOnInit() {
  }
close(){
  this.dialogRef.close();
}

}
