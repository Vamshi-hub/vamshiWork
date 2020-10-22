import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-spinner-dlg',
  templateUrl: './spinner-dlg.component.html',
  styleUrls: ['./spinner-dlg.component.css']
})
export class SpinnerDlgComponent {


  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

}
