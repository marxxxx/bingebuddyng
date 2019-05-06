import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';
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
    console.log('confirmation dialog created');
    console.log(data);
  }

  //////////////////////////////////////////////////////////////////////////////
  // functions
  //////////////////////////////////////////////////////////////////////////////
  ngOnInit() {
  }

}
