import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { environment } from 'src/environments/environment';
import { PersonalUsagePerWeekdayDTO } from './../../../models/PersonalUsagePerWeekdayDTO';
import { UserStatisticHistoryDTO } from './UserStatisticHistoryDTO';

@Injectable()
export class StatisticsService {

  readonly baseUrl = environment.BaseDataUrl + '/statistics';

  constructor(private http: HttpClient) { }

  getStatisticsForUser(userId: string): Observable<UserStatisticHistoryDTO[]> {
    const url = `${this.baseUrl}/${userId}`;
    return this.http.get<UserStatisticHistoryDTO[]>(url);
  }

  getPersonalUsagePerWeekDay(userId: string): Observable<PersonalUsagePerWeekdayDTO[]> {
    const url = `${this.baseUrl}/${userId}/personalusageperweekday`;
    return this.http.get<PersonalUsagePerWeekdayDTO[]>(url);
  }
}
