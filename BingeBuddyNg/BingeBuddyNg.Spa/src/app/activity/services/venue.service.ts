import { Location } from '../../../models/Location';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { VenueDTO } from '../../../models/VenueDTO';

@Injectable({providedIn: 'root'})
export class VenueService {

  baseUrl = environment.BaseDataUrl + '/venue';

  constructor(private http: HttpClient) { }

  searchVenues(location: Location): Observable<VenueDTO[]> {
    const url = `${this.baseUrl}?latitude=${location.latitude}&longitude=${location.longitude}`;
    return this.http.get<VenueDTO[]>(url);
  }

  updateCurrentVenue(venue: VenueDTO): Observable<{}> {
    const url = `${this.baseUrl}/update-current`;
    return this.http.post(url, venue);
  }

  resetCurrentVenue(): Observable<{}> {
    const url = `${this.baseUrl}/reset-current`;
    return this.http.post(url, {});
  }
}
