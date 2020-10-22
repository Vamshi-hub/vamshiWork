import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { MaterialMaster } from '../../classes/material-master';

@Component({
  selector: 'app-show-qrcode',
  templateUrl: './show-qrcode.component.html',
  styleUrls: ['./show-qrcode.component.css']
})
export class ShowQrcodeComponent implements OnInit {
  isLoading = true;
  constructor(private dialogRef: MatDialogRef<ShowQrcodeComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {  
  }
  ngOnInit() {
  }

}
