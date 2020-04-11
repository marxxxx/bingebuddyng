import { PagedQueryResult } from '../../../models/PagedQueryResult';
import { ActivityStatsDTO } from '../../../models/ActivityStatsDTO';
import { AddDrinkActivityDTO } from '../../../models/AddDrinkActivityDTO';
import { ActivityAggregationDTO } from '../../../models/ActivityAggregationDTO';
import { ActivityDTO } from '../../../models/ActivityDTO';
import { Observable } from 'rxjs';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { AddMessageActivityDTO } from '../../../models/AddMessageActivityDTO';
import { Location } from 'src/models/Location';
import { AddReactionDTO } from 'src/models/AddReactionDTO';

@Injectable()
export class ActivityService {

  baseUrl = environment.BaseDataUrl + '/activity';

  constructor(private http: HttpClient) { }

  getActivityFeed(activityId: string, continuationToken: string): Observable<PagedQueryResult<ActivityStatsDTO>> {
    const url = `${this.baseUrl}/feed`;
    const params = new HttpParams()
      .set('activityId', activityId)
      .set('continuationToken', continuationToken);

    return this.http.get<PagedQueryResult<ActivityStatsDTO>>(url, { params: params });
  }

  getActivitysForMap(): Observable<ActivityDTO[]> {
    const url = `${this.baseUrl}/map`;

    return this.http.get<ActivityDTO[]>(url);
  }

  getActivityAggregation(userId: string): Observable<ActivityAggregationDTO[]> {
    const url = `${this.baseUrl}/aggregate/${userId}`;
    return this.http.get<ActivityAggregationDTO[]>(url);
  }

  addMessageActivity(activity: AddMessageActivityDTO): Observable<string> {
    const url = `${this.baseUrl}/message`;
    return this.http.post<string>(url, activity);
  }

  addDrinkActivity(activity: AddDrinkActivityDTO): Observable<string> {
    const url = `${this.baseUrl}/drink`;
    return this.http.post<string>(url, activity);
  }

  addReaction(activityId: string, reaction: AddReactionDTO) {
    const url = `${this.baseUrl}/${activityId}/reaction`;
    return this.http.post(url, reaction);
  }

  deleteActivity(activityId: string): Observable<any> {
    const url = `${this.baseUrl}/${activityId}`;
    return this.http.delete(url);
  }

  getAddImageActivityUrl(loc: Location): string {
    let url = `${this.baseUrl}/image/`;
    if (loc) {
      url += `${loc.latitude}/${loc.longitude}`;
    } else {
      url += '//';
    }

    return url;
  }
}
