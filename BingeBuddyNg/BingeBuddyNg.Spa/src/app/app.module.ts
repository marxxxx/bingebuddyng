import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { MeComponent } from './components/me/me.component';
import { ActivityService } from './services/activity.service';
import { DrinkEventService } from './services/drinkevent.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';


import { AppRoutingModule } from './common/app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavShellComponent } from './core/nav-shell/nav-shell.component';
import { LayoutModule } from '@angular/cdk/layout';
import { WelcomeComponent } from './pages/welcome/welcome.component';
import { ActivityFeedComponent } from './pages/activity-feed/activity-feed.component';
import { StatsComponent } from './pages/stats/stats.component';
import { CallbackComponent } from './components/callback/callback.component';

import {
  HttpClientModule
} from '@angular/common/http';
import 'hammerjs';
import { ChartsModule } from 'ng2-charts';
import { UserService } from './services/user.service';
import { BingemapComponent } from './pages/bingemap/bingemap.component';
import { AgmCoreModule } from '@agm/core';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { EcoFabSpeedDialModule } from '@ecodev/fab-speed-dial';
import { ActivityComponent } from './components/activity/activity.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FileUploadModule } from 'ng2-file-upload';
import { NotificationService } from './services/notification.service';
import { InViewportModule } from 'ng-in-viewport';
import { DrinkChartComponent } from './pages/stats/drink-chart/drink-chart.component';
import { DrinkRatioChartComponent } from './pages/stats/drink-ratio-chart/drink-ratio-chart.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { FriendrequestsComponent } from './pages/friendrequests/friendrequests.component';
import { FriendRequestService } from './services/friendrequest.service';
import { DrinkersComponent } from './pages/drinkers/drinkers.component';
import { DrinkDialogComponent } from './components/drink-dialog/drink-dialog.component';
import { NoFriendsComponent } from './components/no-friends/no-friends.component';
import { MessageDialogComponent } from './components/message-dialog/message-dialog.component';
import { InviteFriendComponent } from './pages/invite-friend/invite-friend.component';
import { WelcomeInvitedComponent } from './pages/welcome-invited/welcome-invited.component';
import { VenueDialogComponent } from './components/venue-dialog/venue-dialog.component';
import { SettingsComponent } from './pages/settings/settings.component';
import { DrinkEventCounterComponent } from './components/drink-event-counter/drink-event-counter.component';
import { OnboardingComponent } from './pages/onboarding/onboarding.component';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [
    AppComponent,
    NavShellComponent,
    WelcomeComponent,
    ActivityFeedComponent,
    MeComponent,
    StatsComponent,
    CallbackComponent,
    BingemapComponent,
    ActivityComponent,
    DrinkChartComponent,
    DrinkRatioChartComponent,
    ProfileComponent,
    FriendrequestsComponent,
    DrinkersComponent,
    DrinkDialogComponent,
    NoFriendsComponent,
    MessageDialogComponent,
    InviteFriendComponent,
    WelcomeInvitedComponent,
    VenueDialogComponent,
    ConfirmationDialogComponent,
    SettingsComponent,
    DrinkEventCounterComponent,
    OnboardingComponent
  ],
  entryComponents: [
    DrinkDialogComponent,
    MessageDialogComponent,
    VenueDialogComponent,
    ConfirmationDialogComponent
  ],
  imports: [
    BrowserModule,
    InViewportModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    ChartsModule,
    EcoFabSpeedDialModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyBlGceFBW7ykKMzNH4o0DwMBlxwt8NgWc8'
    }),
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    CoreModule,
    SharedModule,
    ServiceWorkerModule.register('ngsw-worker.js', {
      enabled: environment.production
    }),
    FileUploadModule
  ],
  providers: [
    DrinkEventService,
    UserService,
    ActivityService,
    NotificationService,
    FriendRequestService
  ],
  bootstrap: [AppComponent]
})
export class AppModule {}
