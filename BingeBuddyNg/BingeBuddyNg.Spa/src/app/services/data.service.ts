import { Observable } from 'rxjs';
import { AuthService } from './auth.service';
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  constructor(private http: HttpClient, private auth: AuthService) { }


  getValues(): Observable<string[]> {
    return this.http.get<string[]>(environment.BaseDataUrl + '/values');

  }

}
