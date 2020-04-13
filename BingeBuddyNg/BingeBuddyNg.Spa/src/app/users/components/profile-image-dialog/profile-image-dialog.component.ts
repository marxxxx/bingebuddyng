import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Inject } from '@angular/core';

import { ProfileImageDialogArgs } from './ProfileImageDialogArgs';

@Component({
  selector: 'app-profile-image-dialog',
  templateUrl: './profile-image-dialog.component.html',
  styleUrls: ['./profile-image-dialog.component.css']
})
export class ProfileImageDialogComponent implements OnInit {

  constructor(@Inject(MAT_DIALOG_DATA) public args: ProfileImageDialogArgs) { }

  ngOnInit() {
  }

}
