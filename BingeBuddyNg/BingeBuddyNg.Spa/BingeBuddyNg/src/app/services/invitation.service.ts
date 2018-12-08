import { Observable } from 'rxjs';
import { environment } from './../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { InvitationInfo } from 'src/models/InvitationInfo';

@Injectable({
  providedIn: 'root'
})
export class InvitationService {

  baseUrl = environment.BaseDataUrl + '/invitation';

  constructor(private http: HttpClient) { }

  getInvitationInfo(invitationToken: string): Observable<InvitationInfo> {
    const url = `${this.baseUrl}/${invitationToken}`;
    return this.http.get<InvitationInfo>(url);
  }

  createInvitation(): Observable<string> {
    return this.http.post<string>(this.baseUrl, {});
  }

  acceptInvitation(invitationToken: string): Observable<string> {
    const url = `${this.baseUrl}/${invitationToken}`;
    return this.http.put<string>(url, {});
  }
}
