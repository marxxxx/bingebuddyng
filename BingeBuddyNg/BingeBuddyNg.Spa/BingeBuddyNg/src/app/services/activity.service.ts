import { ActivityAggregationDTO } from './../../models/ActivityAggregationDTO';
import { ActivityDTO } from '../../models/ActivityDTO';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { AddMessageActivityDTO } from '../../models/AddMessageActivityDTO';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {

  baseUrl = environment.BaseDataUrl + '/activity';

  constructor(private http: HttpClient) { }

  getActivitys(): Observable<ActivityDTO[]> {
    return this.http.get<ActivityDTO[]>(this.baseUrl);
  }

  getActivityAggregation(): Observable<ActivityAggregationDTO[]> {
    const url = `${this.baseUrl}/GetActivityAggregation`;
    return this.http.get<ActivityAggregationDTO[]>(url);
  }

  addActivity(activity: AddMessageActivityDTO): Observable<{}> {
    return this.http.post(this.baseUrl, activity);
  }



}
