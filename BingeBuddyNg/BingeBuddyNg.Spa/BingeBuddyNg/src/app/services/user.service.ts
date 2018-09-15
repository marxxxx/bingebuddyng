import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Injectable } from '@angular/core';
import { User } from '../../models/User';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.BaseDataUrl + '/user';

  constructor(private http: HttpClient) { }

  getUser(userId: string): Observable<User> {
    return this.http.get<User>(`${this.baseUrl}/${userId}`);
  }

  addFriend(friendUserId: string): Observable<{}> {
    return this.http.put(`${this.baseUrl}/{friendUserId}/add`, {});
  }


  removeFriend(friendUserId: string): Observable<{}> {
    return this.http.delete(`${this.baseUrl}/${friendUserId}`, {});
  }

  saveUser(user: User): Observable<{}> {
    console.log('registering user');
    console.log(user);
    return this.http.post(this.baseUrl, user);
  }
}
