import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, OnInit, Inject } from '@angular/core';
import { ActivityDTO } from 'src/models/ActivityDTO';

@Component({
  selector: 'app-reaction-dialog',
  templateUrl: './reaction-dialog.component.html',
  styleUrls: ['./reaction-dialog.component.scss']
})
export class ReactionDialogComponent implements OnInit {

  selectedTabIndex = 0;

  constructor(@Inject(MAT_DIALOG_DATA) public data: ActivityDTO) { }

  ngOnInit() {
    if (this.data.cheers.length > this.data.likes.length) {
      this.selectedTabIndex = 1;
    }
  }

}
