import { Injectable, EventEmitter } from '@angular/core';

@Injectable({providedIn: 'root'})
export class NotificationService {

  activityReceived$: EventEmitter<any> = new EventEmitter();

  constructor() { }

  raiseActivityReceived(): void {
    this.activityReceived$.next();
  }

}
