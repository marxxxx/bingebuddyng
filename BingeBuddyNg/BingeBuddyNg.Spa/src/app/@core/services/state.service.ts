import { Injectable, EventEmitter } from '@angular/core';
import { ActivityStatsDTO } from 'src/models/ActivityStatsDTO';

@Injectable({providedIn: 'root'})
export class StateService {

  private pendingFriendRequestsChangedSource =  new EventEmitter();
  pendingFriendRequestsChanged$ = this.pendingFriendRequestsChangedSource.asObservable();

  private activityReceivedSource = new EventEmitter<ActivityStatsDTO>();
  activityReceived$ = this.activityReceivedSource.asObservable();

  constructor() { }

  raisePendingFriendRequestsChanged(): void {
    this.pendingFriendRequestsChangedSource.emit();
  }

  raiseActivityReceived(activity: ActivityStatsDTO): void {
    this.activityReceivedSource.emit(activity);
  }
}
