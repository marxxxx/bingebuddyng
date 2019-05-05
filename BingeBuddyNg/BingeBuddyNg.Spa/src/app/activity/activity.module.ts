import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { DrinkDialogComponent } from './drink-dialog/drink-dialog.component';
import { MessageDialogComponent } from './message-dialog/message-dialog.component';
import { VenueDialogComponent } from './venue-dialog/venue-dialog.component';
import { ActivityService } from './activity.service';
import { ActivityFeedComponent } from './activity-feed/activity-feed.component';
import { ActivityComponent } from './activity/activity.component';
import { NoFriendsComponent } from './no-friends/no-friends.component';
import { InViewportModule } from 'ng-in-viewport';
import { EcoFabSpeedDialModule } from '@ecodev/fab-speed-dial';
import { RouterModule } from '@angular/router';
import { DrinkRetrieverService } from './drink-retriever.service';

@NgModule({
  imports: [SharedModule, InViewportModule, EcoFabSpeedDialModule, RouterModule],
  exports: [],
  declarations: [
    ActivityFeedComponent,
    ActivityComponent,
    NoFriendsComponent,
    DrinkDialogComponent,
    MessageDialogComponent,
    VenueDialogComponent
  ],
  entryComponents: [
    DrinkDialogComponent,
    MessageDialogComponent,
    VenueDialogComponent
  ],
  providers: [ActivityService, DrinkRetrieverService]
})
export class ActivityModule {}
