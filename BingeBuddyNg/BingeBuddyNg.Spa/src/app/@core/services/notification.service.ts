import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

import { ActivityStatsDTO } from 'src/models/ActivityStatsDTO';
import { environment } from 'src/environments/environment';
import { AuthService } from './auth.service';

@Injectable({ providedIn: 'root' })
export class NotificationService {

  readonly hubName = 'Notification';
  readonly methodName = 'activity-received';
  private connection: signalR.HubConnection;

  private activityReceivedSource = new Subject<ActivityStatsDTO>();

  activityReceived$ = this.activityReceivedSource.asObservable();

  constructor(private auth: AuthService) { }

  start(): Promise<void> {

    const url = `${environment.BaseDataUrl}/Negotiation/${this.hubName}`;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(url, { accessTokenFactory: () => this.auth.getAccessToken() })
      .build();

    this.connection.on(this.methodName, (payload) => {
      console.log(payload);
      this.activityReceivedSource.next(JSON.parse(payload));
    });

    return this.connection.start();
  }

  stop(): Promise<void> {
    return this.connection.stop();
  }
}
