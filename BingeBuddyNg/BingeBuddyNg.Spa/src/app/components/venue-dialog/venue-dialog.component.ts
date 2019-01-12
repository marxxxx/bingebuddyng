import { VenueService } from './../../services/venue.service';
import { Component, OnInit, Inject } from '@angular/core';
import { VenueModel } from 'src/models/VenueModel';
import { Observable } from 'rxjs';
import { MAT_DIALOG_DATA } from '@angular/material';
import { VenueDialogArgs } from './VenueDialogArgs';

@Component({
  selector: 'app-venue-dialog',
  templateUrl: './venue-dialog.component.html',
  styleUrls: ['./venue-dialog.component.css']
})
export class VenueDialogComponent implements OnInit {

  venues$: Observable<VenueModel[]>;
  selectedVenue: VenueModel;

  constructor(
    @Inject(MAT_DIALOG_DATA) args: VenueDialogArgs,
    venueService: VenueService) {
    this.venues$ = venueService.getVenues(args.location);
  }

  ngOnInit() {
  }

}