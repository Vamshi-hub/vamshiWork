import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';

@Component({
  selector: 'app-material-slide-show',
  templateUrl: './slide-show.component.html',
  styleUrls: ['./slide-show.component.css']
})
export class MaterialSlideShowComponent implements OnInit {

  constructor(private dialogRef: MatDialogRef<MaterialSlideShowComponent>, @Inject(MAT_DIALOG_DATA) public data: any) { }

  ngOnInit() {
   
  }


  close() {
    this.dialogRef.close();
  }
}
