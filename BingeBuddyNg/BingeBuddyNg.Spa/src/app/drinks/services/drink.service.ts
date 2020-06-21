import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { DrinkDTO } from 'src/models/DrinkDTO';
import { Observable } from 'rxjs';


@Injectable()
export class DrinkService {

  baseUrl = environment.BaseDataUrl + '/drink';

  constructor(private http: HttpClient) { }

  getDrinks(): Observable<DrinkDTO[]> {
    return this.http.get<DrinkDTO[]>(this.baseUrl);
  }

  getDrink(drinkId: string): Observable<DrinkDTO> {
    return this.http.get<DrinkDTO>(`${this.baseUrl}/${drinkId}`);
  }

  saveDrinks(drinks: DrinkDTO[]): Observable<any> {
    return this.http.post(this.baseUrl, drinks);
  }

  deleteDrink(drinkId: string): Observable<any> {
    const url = `${this.baseUrl}/${drinkId}`;
    return this.http.delete(url);
  }
}
