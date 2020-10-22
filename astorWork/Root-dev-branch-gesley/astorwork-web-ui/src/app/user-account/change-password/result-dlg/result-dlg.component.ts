import { Component, Inject } from '@angular/core';
import { MatDialog, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-change-password-result-dlg',
  templateUrl: './result-dlg.component.html',
  styleUrls: ['./result-dlg.component.css']
})
export class ChangePwDlgComponent {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any) { }

}
