import { VenueRankingDTO } from '../../../models/VenueRankingDTO';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { UserRankingDTO } from 'src/models/UserRankingDTO';

@Injectable()
export class RankingService {

  baseUrl = environment.BaseDataUrl + '/ranking';

  constructor(private http: HttpClient) { }

  getRanking(): Observable<UserRankingDTO[]> {
    const url = `${this.baseUrl}/GetDrinkRanking`;
    return this.http.get<UserRankingDTO[]>(url);
  }

  getScores(): Observable<UserRankingDTO[]> {
    const url = `${this.baseUrl}/GetScoreRanking`;
    return this.http.get<UserRankingDTO[]>(url);
  }

  getVenueRanking(): Observable<VenueRankingDTO[]> {
    const url = `${this.baseUrl}/GetVenueRanking`;
    return this.http.get<VenueRankingDTO[]>(url);
  }
}
