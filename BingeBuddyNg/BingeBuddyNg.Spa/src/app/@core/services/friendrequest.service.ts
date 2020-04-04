import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { FriendRequestDTO } from '../../../models/FriendRequestDTO';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class FriendRequestService {

  baseUrl = environment.BaseDataUrl + '/friendrequest';

  constructor(private http: HttpClient) { }

  getPendingFriendRequests(): Observable<FriendRequestDTO[]> {
    const url = `${this.baseUrl}/pending`;
    return this.http.get<FriendRequestDTO[]>(url);
  }

  hasPendingFriendRequests(userId: string): Observable<boolean> {
    const url = `${this.baseUrl}/hasPending/${userId}`;
    return this.http.get<boolean>(url);
  }

  addFriendRequest(userId: string): Observable<any> {
    const url = `${this.baseUrl}/request/${userId}`;
    return this.http.post(url, {});
  }

  acceptFriendRequest(userId: string): Observable<any> {
    const url = `${this.baseUrl}/accept/${userId}`;
    return this.http.put(url, {});
  }

  declineFriendRequest(userId: string): Observable<any> {
    const url = `${this.baseUrl}/decline/${userId}`;
    return this.http.put(url, {});
  }
}
