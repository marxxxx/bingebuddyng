import { LocationService } from 'src/app/activity/services/location.service';
import { UserInfoDTO } from '../../../../models/UserInfoDTO';
import { ActivatedRoute } from '@angular/router';
import { ActivityDTO } from '../../../../models/ActivityDTO';
import { ActivityService } from '../../../activity/services/activity.service';
import { Location } from '../../../../models/Location';
import { Component, OnInit, ViewChild } from '@angular/core';

declare var google: any;

@Component({
  selector: 'app-bingemap',
  templateUrl: './bingemap.component.html',
  styleUrls: ['./bingemap.component.css']
})
export class BingemapComponent implements OnInit {

  selectedActivityId: string;
  location: Location;
  isBusy = false;
  activitys: ActivityDTO[] = [];

  @ViewChild('AgmMap', { static: true }) agmMap: any;

  constructor(private activityService: ActivityService, private locationService: LocationService,
    private route: ActivatedRoute) { }

  ngOnInit() {

    this.locationService.getLocation().then(l => {
      this.location = l;
    }).catch(e => {
      console.error('no location available.');
    });

    this.route.queryParamMap.subscribe(p => {
      this.selectedActivityId = p.get('selectedActivityId');
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

  getUserInfo(a: ActivityDTO): UserInfoDTO {
    const userInfo: UserInfoDTO = {
        userId: a.userId,
        userName: a.userName
    };

    return userInfo;
}

  private fitBounds() {
    const bounds = new google.maps.LatLngBounds();
    for (const mm of this.activitys) {
      bounds.extend({ lat: mm.location.latitude, lng: mm.location.longitude });
    }
    this.agmMap.fitBounds = bounds;
    console.log('fitted bounds');
    this.agmMap.triggerResize();
  }
}
