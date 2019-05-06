import { Location } from '../../../models/Location';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { VenueModel } from '../../../models/VenueModel';

@Injectable({providedIn: 'root'})
export class VenueService {

  baseUrl = environment.BaseDataUrl + '/venue';

  constructor(private http: HttpClient) { }

  getVenues(location: Location): Observable<VenueModel[]> {
    const url = `${this.baseUrl}?latitude=${location.latitude}&longitude=${location.longitude}`;
    return this.http.get<VenueModel[]>(url);
  }

  updateCurrentVenue(venue: VenueModel): Observable<{}> {
    const url = `${this.baseUrl}/UpdateCurrentVenue`;
    return this.http.post(url, venue);
  }

  resetCurrentVenue(): Observable<{}> {
    const url = `${this.baseUrl}/ResetCurrentVenue`;
    return this.http.post(url, {});
  }
}
