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

  constructor() { }

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


  setCurrentLocation(location: LocationDTO) {
    localStorage.setItem(this.locationStorageKey, JSON.stringify(location));
  }

  getCurrentLocation(): LocationDTO {
    const location = localStorage.getItem(this.locationStorageKey);
    if (location == null) {
      return null;
    }

    return <LocationDTO>JSON.parse(location);
  }
}
