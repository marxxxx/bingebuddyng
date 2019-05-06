import { NgModule } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { DrinkDialogComponent } from './components/drink-dialog/drink-dialog.component';
import { MessageDialogComponent } from './components/message-dialog/message-dialog.component';
import { VenueDialogComponent } from './components/venue-dialog/venue-dialog.component';
import { ActivityService } from './services/activity.service';
import { ActivityFeedComponent } from './components/activity-feed/activity-feed.component';
import { ActivityComponent } from './components/activity/activity.component';
import { NoFriendsComponent } from './components/no-friends/no-friends.component';
import { InViewportModule } from 'ng-in-viewport';
import { EcoFabSpeedDialModule } from '@ecodev/fab-speed-dial';
import { RouterModule } from '@angular/router';
import { DrinkRetrieverService } from './services/drink-retriever.service';

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
