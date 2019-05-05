import { VenueService } from '../activity/venue.service';
import { BehaviorSubject, Subject, Observable } from 'rxjs';
import { VenueModel } from '../../models/VenueModel';
import { Location } from 'src/models/Location';
import { Injectable } from '@angular/core';

/**
 * Provides geo-location related utility functions.
 */
@Injectable()
export class LocationService {

  private readonly locationStorageKey = 'bingebuddy:location';
  private previousLocation: Location;

  currentLocation$ = new BehaviorSubject<Location>(null);
  locationChanged$ = new Subject();

  currentVenue: VenueModel;

  constructor(private venueService: VenueService) { }


  refreshLocation(currentVenue: VenueModel) {
    this.previousLocation = this.loadCurrentLocation();
    console.log('refreshing location');
    this.getLocation().then(l => {
      this.currentLocation$.next(l);

      if (this.hasVenueLocationChanged(this.previousLocation, this.currentLocation$.value, currentVenue)) {
        this.locationChanged$.next();
      }

      this.storeCurrentLocation(this.currentLocation$.value);
    }, e => {
      console.error('error retrieving location');
      console.error(e);
    });
  }

  setCurrentVenue(venue: VenueModel): Observable<{}> {
    this.currentVenue = venue;
    this.refreshLocation(venue);

    // update venue for user in backend
    return this.venueService.updateCurrentVenue(venue);
  }

  resetCurrentVenue(): Observable<{}> {
    this.currentVenue = null;
    return this.venueService.resetCurrentVenue();
  }


  getCurrentVenue(): VenueModel {
    return this.currentVenue;
  }

  hasVenueLocationChanged(previousLocation: Location, currentLocation: Location, currentVenue: VenueModel): boolean {

    if (!currentVenue) {
      return false;
    }
    const hasBothLocations = previousLocation != null && currentLocation != null;
    if (hasBothLocations === false) {
      return false;
    }

    const latDiff = Math.abs(previousLocation.latitude - currentLocation.latitude);
    const lngDiff = Math.abs(previousLocation.longitude - currentLocation.longitude);

    const hasLocationChanged = (latDiff > 0.01 || lngDiff > 0.01);
    return hasLocationChanged;
  }

  storeCurrentLocation(location: Location) {
    console.log('storeCurrentLocation', location);
    localStorage.setItem(this.locationStorageKey, JSON.stringify(location));
  }

  loadCurrentLocation(): Location {
    const location = localStorage.getItem(this.locationStorageKey);
    if (location == null) {
      console.warn('current location not found');
      return null;
    }

    console.log('current location loaded', location);
    return JSON.parse(location);
  }

  getCurrentLocation(): Location {
    return this.currentLocation$.value;
  }


  getLocation(): Promise<Location> {

    const promise = new Promise<Location>((resolve, reject) => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((position) => {
          const result = new Location(position.coords.latitude, position.coords.longitude);
          resolve(result);
        });
      } else {
        reject('GeoLocation not supported.');
      }
    });

    return promise;
  }

  hasLocation(): boolean {
    return this.currentLocation$.value != null;
  }
}
