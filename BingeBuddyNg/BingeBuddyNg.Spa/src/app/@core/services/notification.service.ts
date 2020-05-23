import { Injectable } from '@angular/core';
import { Subject, BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';

import { ActivityStatsDTO } from 'src/models/ActivityStatsDTO';
import { environment } from 'src/environments/environment';
import { AuthService } from './auth/auth.service';
import { filter } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class NotificationService {

  readonly hubName = 'Notification';
  readonly methodName = 'activity-received';
  private connection: signalR.HubConnection;

  private activityReceivedSource = new Subject<ActivityStatsDTO>();

  activityReceived$ = this.activityReceivedSource.asObservable();

  private initialized = new BehaviorSubject<boolean>(false);

  constructor(private auth: AuthService) { }

  async start(): Promise<void> {

    const url = `${environment.BaseDataUrl}/Negotiation/${this.hubName}`;

    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(url, { accessTokenFactory: () => this.auth.getAccessToken() })
      .build();

    this.connection.on(this.methodName, (payload) => {
      console.log(payload);
      this.activityReceivedSource.next(JSON.parse(payload));
    });

    await this.connection.start();

    this.initialized.next(true);
  }

  stop(): Promise<void> {
    return this.connection.stop();
  }

  on(methodName: string, callback: (payload) => void) {
    this.initialized.pipe(filter(isInitialized => isInitialized))
      .subscribe(() => {
        this.connection.on(methodName, (nativePayload) => {
          callback(JSON.parse(nativePayload));
        });
      });

  }
}
