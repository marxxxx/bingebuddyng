import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { UserDTO } from '../../models/UserDTO';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.BaseDataUrl + '/user';

  constructor(private http: HttpClient) { }

  saveUser(user: UserDTO): Observable<{}> {
    console.log('registering user');
    console.log(user);
    return this.http.post(this.baseUrl, user);
  }
}
