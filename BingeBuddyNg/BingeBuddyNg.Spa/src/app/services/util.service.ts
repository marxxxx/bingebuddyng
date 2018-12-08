import { Injectable } from '@angular/core';
import { LocationDTO } from '../../models/LocationDTO';

@Injectable({
  providedIn: 'root'
})
export class UtilService {

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
}
