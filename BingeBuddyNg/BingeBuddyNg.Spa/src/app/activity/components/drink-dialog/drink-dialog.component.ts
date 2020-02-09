import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DrinkDialogArgs } from './DrinkDialogArgs';
import { DrinkType } from '../../../../models/DrinkType';

@Component({
  selector: 'app-drink-dialog',
  templateUrl: './drink-dialog.component.html',
  styleUrls: ['./drink-dialog.component.css']
})
export class DrinkDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public args: DrinkDialogArgs,
    public dialogRef: MatDialogRef<DrinkDialogComponent>) { }

  ngOnInit() {
    setTimeout(() => this.dialogRef.close(), 7000);
  }

  onClose() {
    this.dialogRef.close();
  }
}
