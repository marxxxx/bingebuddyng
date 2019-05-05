import { Injectable, EventEmitter } from '@angular/core';

@Injectable()
export class NotificationService {

  activityReceived$: EventEmitter<any> = new EventEmitter();

  constructor() { }

  raiseActivityReceived(): void {
    this.activityReceived$.next();
  }

}
