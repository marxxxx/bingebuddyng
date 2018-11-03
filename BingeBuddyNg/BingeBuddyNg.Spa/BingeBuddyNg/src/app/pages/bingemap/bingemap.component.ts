import { UserInfo } from './../../../models/UserInfo';
import { ActivatedRoute } from '@angular/router';
import { Activity } from '../../../models/Activity';
import { ActivityService } from '../../services/activity.service';
import { LocationDTO } from '../../../models/LocationDTO';
import { Subject } from 'rxjs';
import { UtilService } from '../../services/util.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AgmMap, LatLngBounds, LatLng, AgmMarker } from '@agm/core';
import * as moment from 'moment';
import { } from 'googlemaps';

@Component({
  selector: 'app-bingemap',
  templateUrl: './bingemap.component.html',
  styleUrls: ['./bingemap.component.css']
})
export class BingemapComponent implements OnInit {

  location: LocationDTO;
  isBusy = false;
  activitys: Activity[] = [];

  @ViewChild('AgmMap') agmMap: any;

  constructor(private activityService: ActivityService, private util: UtilService,
    private route: ActivatedRoute) { }

  ngOnInit() {

    this.util.getLocation().then(l => {
      this.location = l;
    }).catch(e => {
      console.error('no location available.');
    });

    this.route.params.subscribe(p => {
      this.load();
    });
  }


  load() {
    this.isBusy = true;
    this.activityService.getActivitys({ onlyWithLocation: true }).subscribe(d => {
      this.activitys = d;
      this.isBusy = false;

      // fit bounds
      setTimeout(() => this.fitBounds(), 1500);

    }, e => {
      this.isBusy = false;
      console.error(e);
    });
  }

  getUserInfo(a: Activity): UserInfo {
    const userInfo: UserInfo = {
        userId: a.userId,
        userName: a.userName
    };

    return userInfo;
}

  private fitBounds() {
    const bounds: google.maps.LatLngBounds = new google.maps.LatLngBounds();
    for (const mm of this.activitys) {
      bounds.extend({ lat: mm.location.latitude, lng: mm.location.longitude });
    }
    this.agmMap.fitBounds = bounds;
    console.log('fitted bounds');
    this.agmMap.triggerResize();
  }
}
