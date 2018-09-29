import { UserInfo } from './../../models/UserInfo';
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

  getAllUsers(filterText: string): Observable<UserInfo[]> {
    let url = this.baseUrl;
    if (filterText) {
      url += encodeURI('?filterText=' + filterText);
    }
    return this.http.get<UserInfo[]>(url);
  }

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
