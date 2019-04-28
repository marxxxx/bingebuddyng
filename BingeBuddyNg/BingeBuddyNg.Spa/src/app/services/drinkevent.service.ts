import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { Injectable, EventEmitter } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { DrinkEvent } from 'src/models/DrinkEvent';

@Injectable({
  providedIn: 'root'
})
export class DrinkEventService {

  currentUserScored$ = new EventEmitter();

  constructor(private http: HttpClient) { }

  getCurrentDrinkEvent(): Observable<DrinkEvent> {
    return this.http.get<DrinkEvent>(environment.BaseDataUrl + '/drinkevent/current');
  }

}
