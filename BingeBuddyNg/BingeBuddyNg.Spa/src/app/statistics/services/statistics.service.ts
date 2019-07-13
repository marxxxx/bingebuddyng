import { UserStatisticHistoryDTO } from './UserStatisticHistoryDTO';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable()
export class StatisticsService {

  readonly baseUrl = environment.BaseDataUrl + '/statistics';

  constructor(private http: HttpClient) { }

  getStatisticsForUser(userId: string): Observable<UserStatisticHistoryDTO[]> {
    const url = `${this.baseUrl}/${userId}`;
    return this.http.get<UserStatisticHistoryDTO[]>(url);
  }
}
