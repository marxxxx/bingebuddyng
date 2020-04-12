import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ConfirmationDialogArgs } from './ConfirmationDialogArgs';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.scss']
})
export class ConfirmationDialogComponent implements OnInit {

  //////////////////////////////////////////////////////////////////////////////
  // construction
  //////////////////////////////////////////////////////////////////////////////
  constructor(@Inject(MAT_DIALOG_DATA) public data: ConfirmationDialogArgs) {
  }

  //////////////////////////////////////////////////////////////////////////////
  // functions
  //////////////////////////////////////////////////////////////////////////////
  ngOnInit() {
  }

}
