import { UserInfoDTO } from '../../../models/UserInfoDTO';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Injectable } from '@angular/core';
import { UserDTO } from '../../../models/UserDTO';
import { CreateOrUpdateUserDTO } from 'src/models/CreateOrUpdateUserDTO';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  baseUrl = environment.BaseDataUrl + '/user';
  timestamp = new Date().toISOString();

  constructor(private http: HttpClient) {
  }

  getAllUsers(filterText: string): Observable<UserInfoDTO[]> {
    let url = this.baseUrl;
    if (filterText) {
      url += encodeURI('?filterText=' + filterText);
    }
    return this.http.get<UserInfoDTO[]>(url);
  }

  getUser(userId: string): Observable<UserDTO> {
    return this.http.get<UserDTO>(`${this.baseUrl}/${userId}`);
  }

  addFriendRequest(userId: string): Observable<any> {
    const url = `${this.baseUrl}/${userId}/request`;
    return this.http.post(url, {});
  }

  acceptFriendRequest(userId: string): Observable<any> {
    const url = `${this.baseUrl}/${userId}/accept`;
    return this.http.put(url, {});
  }

  declineFriendRequest(userId: string): Observable<any> {
    const url = `${this.baseUrl}/${userId}/decline`;
    return this.http.put(url, {});
  }

  removeFriend(friendUserId: string): Observable<{}> {
    return this.http.delete(`${this.baseUrl}/${friendUserId}/removefriend`, {});
  }

  createOrUpdateUser(user: CreateOrUpdateUserDTO): Observable<{}> {
    return this.http.post(this.baseUrl, user);
  }

  setFriendMuteState(friendUserId: string, muteState: boolean): Observable<any> {
    const url = `${this.baseUrl}/${friendUserId}/mute?state=${muteState}`;
    return this.http.put(url, {});
  }

  deleteMyself(): Observable<any> {
    const url = `${this.baseUrl}/myself`;
    return this.http.delete(url);
  }

  getProfileImageUrl(userId: string): string {
    return `https://bingebuddystorage.blob.core.windows.net/profileimg/${userId}?t=${this.timestamp}`;
  }

  getUpdateProfileImageUrl(): string {
    return `${this.baseUrl}/profile-image`;
  }
}
