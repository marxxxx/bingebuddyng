import { ActivityStatsDTO } from './../../models/ActivityStatsDTO';
import { AddDrinkActivityDTO } from './../../models/AddDrinkActivityDTO';
import { GetActivityFilterArgs } from '../../models/GetActivityFilterArgs';
import { ActivityAggregationDTO } from '../../models/ActivityAggregationDTO';
import { Activity } from '../../models/Activity';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { AddMessageActivityDTO } from '../../models/AddMessageActivityDTO';
import { ReactionDTO } from 'src/models/ReactionDTO';
import { LocationDTO } from 'src/models/LocationDTO';

@Injectable({
  providedIn: 'root'
})
export class ActivityService {

  baseUrl = environment.BaseDataUrl + '/activity';

  constructor(private http: HttpClient) { }

  getActivityFeed(): Observable<ActivityStatsDTO[]> {
    const url = `${this.baseUrl}/GetActivityFeed`;
    return this.http.get<ActivityStatsDTO[]>(url);
  }

  getActivitys(args: GetActivityFilterArgs): Observable<Activity[]> {
    const url = `${this.baseUrl}/${args.onlyWithLocation}`;

    return this.http.get<Activity[]>(url);
  }

  getActivityAggregation(): Observable<ActivityAggregationDTO[]> {
    const url = `${this.baseUrl}/GetActivityAggregation`;
    return this.http.get<ActivityAggregationDTO[]>(url);
  }

  addMessageActivity(activity: AddMessageActivityDTO): Observable<any> {
    const url = `${this.baseUrl}/AddMessageActivity`;
    return this.http.post(url, activity);
  }

  addDrinkActivity(activity: AddDrinkActivityDTO): Observable<any> {
    const url = `${this.baseUrl}/AddDrinkActivity`;
    return this.http.post(url, activity);
  }

  addReaction(reaction: ReactionDTO) {
    const url = `${this.baseUrl}/AddReaction`;
    return this.http.post(url, reaction);
  }

  getAddImageActivityUrl(loc: LocationDTO): string {
    let url = `${this.baseUrl}/AddImageActivity/`;
    if (loc) {
      url += `${loc.latitude}/${loc.longitude}`;
    } else {
      url += '//';
    }

    return url;
  }
}
