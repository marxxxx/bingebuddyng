import { ActivityDTO } from './../../../models/ActivityDTO';
import { ActivityService } from './../../services/activity.service';
import { LocationDTO } from './../../../models/LocationDTO';
import { Subject } from 'rxjs';
import { UtilService } from './../../services/util.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AgmMap, LatLngBounds, LatLng, AgmMarker } from '@agm/core';
import * as moment from 'moment';
import {} from '@types/googlemaps';

@Component({
  selector: 'app-bingemap',
  templateUrl: './bingemap.component.html',
  styleUrls: ['./bingemap.component.css']
})
export class BingemapComponent implements OnInit {

  location: LocationDTO;
  isBusy = false;
  activitys: ActivityDTO[] = [];

  @ViewChild('AgmMap') agmMap: any;

  constructor(private activityService: ActivityService, private util: UtilService) { }

  ngOnInit() {

    this.util.getLocation().then(l => {
      this.location = l;

    });

    this.load();
  }

  load() {
    this.isBusy = true;
    this.activityService.getActivitys({ onlyWithLocation: true }).subscribe(d => {
      this.activitys = d;
      this.isBusy = false;
      console.log('got data');
      console.log(d);


      // fit bounds
      const bounds: google.maps.LatLngBounds = new google.maps.LatLngBounds();
      for (const mm of this.activitys) {
         bounds.extend({ lat: mm.location.latitude, lng: mm.location.longitude });
       }
      this.agmMap.fitBounds = bounds;
      console.log('fitted bounds');
      this.agmMap.triggerResize();


    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  formatLabel(a: ActivityDTO): string {
    const time = moment(a.timestamp).format('DD.MM.YYYY HH:mm');
    return `${a.userName} : ${time}`;
  }



}
