import { LocationDTO } from '../../models/LocationDTO';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { VenueModel } from '../../models/VenueModel';

@Injectable({
  providedIn: 'root'
})
export class VenueService {

  baseUrl = environment.BaseDataUrl + '/venue';

  constructor(private http: HttpClient) { }

  getVenues(location: LocationDTO): Observable<VenueModel[]> {
    const url = `${this.baseUrl}?latitude=${location.latitude}&longitude=${location.longitude}`;
    return this.http.get<VenueModel[]>(url);
  }

}
