import { Injectable, EventEmitter } from '@angular/core';

@Injectable()
export class StateService {

  private currentUserId: string;
  pendingFriendRequestsChanged$: EventEmitter<any> = new EventEmitter();

  constructor() { }

  raisePendingFriendRequestsChanged(): void {
    this.pendingFriendRequestsChanged$.emit();
  }

  getCurrentUserId(): string {
    return this.currentUserId;
  }

  setCurrentUserId(userId: string) {
    this.currentUserId = userId;
  }

}
