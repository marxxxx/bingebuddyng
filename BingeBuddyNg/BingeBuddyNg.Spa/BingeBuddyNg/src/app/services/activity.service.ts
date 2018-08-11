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

  addActivity(activity: AddMessageActivityDTO): Observable<{}> {
    return this.http.post(this.baseUrl, activity);
  }

}
