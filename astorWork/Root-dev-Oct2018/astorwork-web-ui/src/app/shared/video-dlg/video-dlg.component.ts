import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
import { UiUtilsService } from '../ui-utils.service';

@Component({
  selector: 'app-video-dlg',
  templateUrl: './video-dlg.component.html',
  styleUrls: ['./video-dlg.component.css']
})
export class VideoDlgComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public data: any, private uiUtilService: UiUtilsService) { }

  ngOnInit() {
  }

  onCloseClicked() {
    this.uiUtilService.closeAllDialog();
  }

}
