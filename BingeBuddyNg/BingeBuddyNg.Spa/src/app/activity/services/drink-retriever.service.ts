import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';
import { Drink } from 'src/models/Drink';

@Injectable()
export class DrinkRetrieverService {
  baseUrl = environment.BaseDataUrl + '/drink';

  constructor(private http: HttpClient) {}

  getDrinks(): Observable<Drink[]> {
    return this.http.get<Drink[]>(this.baseUrl);
  }
}
