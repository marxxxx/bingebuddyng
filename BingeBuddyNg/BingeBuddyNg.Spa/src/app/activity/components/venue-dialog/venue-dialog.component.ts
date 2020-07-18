import { VenueService } from '../../services/venue.service';
import { Component, OnInit, Inject } from '@angular/core';
import { VenueDTO } from 'src/models/VenueDTO';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { VenueDialogArgs } from './VenueDialogArgs';
import { VenueDialogMode } from './VenueDialogMode';
import { VenueDialogResult } from './VenueDialogResult';

@Component({
  selector: 'app-venue-dialog',
  templateUrl: './venue-dialog.component.html',
  styleUrls: ['./venue-dialog.component.css']
})
export class VenueDialogComponent implements OnInit {

  isBusy = false;
  venues: VenueDTO[];
  selectedVenue: VenueDTO;

  VenueDialogMode = VenueDialogMode;

  constructor(
    @Inject(MAT_DIALOG_DATA) public args: VenueDialogArgs,
    private dialog: MatDialogRef<VenueDialogComponent>,
    private venueService: VenueService) {

  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.isBusy = true;
    this.venueService.searchVenues(this.args.location)
      .subscribe(r => {
        this.isBusy = false;
        this.venues = r;
      }, e => {
        this.isBusy = false;
        const ret: VenueDialogResult = { action: 'cancel'};
        this.dialog.close(ret);
      });
  }
}
