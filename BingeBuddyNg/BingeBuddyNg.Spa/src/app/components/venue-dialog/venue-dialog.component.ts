import { VenueService } from './../../services/venue.service';
import { Component, OnInit, Inject } from '@angular/core';
import { VenueModel } from 'src/models/VenueModel';
import { Observable } from 'rxjs';
import { MAT_DIALOG_DATA } from '@angular/material';
import { VenueDialogArgs } from './VenueDialogArgs';
import { VenueDialogMode } from './VenueDialogMode';

@Component({
  selector: 'app-venue-dialog',
  templateUrl: './venue-dialog.component.html',
  styleUrls: ['./venue-dialog.component.css']
})
export class VenueDialogComponent implements OnInit {

  venues$: Observable<VenueModel[]>;
  selectedVenue: VenueModel;

  VenueDialogMode = VenueDialogMode;

  constructor(
    @Inject(MAT_DIALOG_DATA) public args: VenueDialogArgs,
    venueService: VenueService) {
    this.venues$ = venueService.getVenues(args.location);
  }

  ngOnInit() {
  }

}
