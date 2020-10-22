import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-sample-view',
  templateUrl: './sample-view.component.html',
  styleUrls: ['./sample-view.component.css']
})
export class SampleViewFileComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<SampleViewFileComponent>) { }

  ngOnInit() {
  }
close(){
  this.dialogRef.close();
}
}