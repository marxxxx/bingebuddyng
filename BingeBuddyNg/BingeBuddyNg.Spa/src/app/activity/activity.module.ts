import { DrinkDialogComponent } from './components/drink-dialog/drink-dialog.component';
import { NgModule } from '@angular/core';
import { SharedModule } from '../@shared/shared.module';
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
import { ReactionDialogComponent } from './components/reaction-dialog/reaction-dialog.component';
import { ReactionListComponent } from './components/reaction-dialog/reaction-list/reaction-list.component';
import { DrinkAnimationComponent } from './components/drink-animation/drink-animation.component';

@NgModule({
  imports: [SharedModule, InViewportModule, EcoFabSpeedDialModule, RouterModule],
  exports: [],
  declarations: [
    ActivityFeedComponent,
    ActivityComponent,
    NoFriendsComponent,
    MessageDialogComponent,
    VenueDialogComponent,
    ReactionDialogComponent,
    ReactionListComponent,
    DrinkAnimationComponent,
    DrinkDialogComponent
  ],
  entryComponents: [MessageDialogComponent, VenueDialogComponent, DrinkDialogComponent, ReactionDialogComponent],
  providers: [ActivityService, DrinkRetrieverService]
})
export class ActivityModule {}
