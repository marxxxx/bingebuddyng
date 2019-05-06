import { VenueRanking } from '../../../models/VenueRanking';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { UserRanking } from 'src/models/UserRanking';

@Injectable()
export class RankingService {

  baseUrl = environment.BaseDataUrl + '/ranking';

  constructor(private http: HttpClient) { }

  getRanking(): Observable<UserRanking[]> {
    const url = `${this.baseUrl}/GetDrinkRanking`;
    return this.http.get<UserRanking[]>(url);
  }

  getScores(): Observable<UserRanking[]> {
    const url = `${this.baseUrl}/GetScoreRanking`;
    return this.http.get<UserRanking[]>(url);
  }

  getVenueRanking(): Observable<VenueRanking[]> {
    const url = `${this.baseUrl}/GetVenueRanking`;
    return this.http.get<VenueRanking[]>(url);
  }
}
