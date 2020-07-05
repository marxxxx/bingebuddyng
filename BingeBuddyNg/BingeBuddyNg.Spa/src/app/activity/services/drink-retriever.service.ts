import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { DrinkDTO } from 'src/models/DrinkDTO';

@Injectable()
export class DrinkRetrieverService {
  baseUrl = environment.BaseDataUrl + '/drink';

  constructor(private http: HttpClient) {}

  getDrinks(): Observable<DrinkDTO[]> {
    return this.http.get<DrinkDTO[]>(this.baseUrl);
  }
}
