import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Drink } from 'src/models/Drink';
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class DrinkService {

  baseUrl = environment.BaseDataUrl + '/drink';

  constructor(private http: HttpClient) { }

  getDrinks(): Observable<Drink[]> {
    return this.http.get<Drink[]>(this.baseUrl);
  }

  saveDrinks(drinks: Drink[]): Observable<any> {
    return this.http.post(this.baseUrl, drinks);
  }

  deleteDrink(drinkId: string) : Observable<any> {
    const url = `${this.baseUrl}/${drinkId}`;
    return this.http.delete(url);
  }
}
