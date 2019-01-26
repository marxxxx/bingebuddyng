import { BehaviorSubject, Subject } from 'rxjs';
import { VenueModel } from './../../models/VenueModel';
import { LocationDTO } from 'src/models/LocationDTO';
import { Injectable } from '@angular/core';

/**
 * Provides geo-location related utility functions.
 */
@Injectable({
  providedIn: 'root'
})
export class LocationService {

  private readonly locationStorageKey = 'bingebuddy:location';
  private previousLocation: LocationDTO;

  currentLocation$ = new BehaviorSubject<LocationDTO>(null);
  locationChanged$ = new Subject();


  constructor() { }


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

  hasVenueLocationChanged(previousLocation: LocationDTO, currentLocation: LocationDTO, currentVenue: VenueModel): boolean {

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

  storeCurrentLocation(location: LocationDTO) {
    console.log('storeCurrentLocation', location);
    localStorage.setItem(this.locationStorageKey, JSON.stringify(location));
  }

  loadCurrentLocation(): LocationDTO {
    const location = localStorage.getItem(this.locationStorageKey);
    if (location == null) {
      console.warn('current location not found');
      return null;
    }

    console.log('current location loaded', location);
    return JSON.parse(location);
  }

  getCurrentLocation(): LocationDTO {
    return this.currentLocation$.value;
  }


  getLocation(): Promise<LocationDTO> {

    const promise = new Promise<LocationDTO>((resolve, reject) => {
      if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition((position) => {
          const result = new LocationDTO(position.coords.latitude, position.coords.longitude);
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
