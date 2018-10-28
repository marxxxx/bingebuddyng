import { MeComponent } from './components/me/me.component';
import { ActivityService } from './services/activity.service';
import { AuthHttpInterceptor } from './services/auth.interceptor';
import { AuthService } from './services/auth.service';
import { DataService } from './services/data.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NavShellComponent } from './components/nav-shell/nav-shell.component';
import { LayoutModule } from '@angular/cdk/layout';
import { WelcomeComponent } from './pages/welcome/welcome.component';
import { ActivityFeedComponent } from './pages/activity-feed/activity-feed.component';
import { StatsComponent } from './pages/stats/stats.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { CallbackComponent } from './components/callback/callback.component';
import { AppMaterialModule } from './app-material/app-material.module';
import { HttpClientModule, HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import 'hammerjs';
import { ChartsModule } from 'ng2-charts';
import { UserService } from './services/user.service';
import { BingemapComponent } from './pages/bingemap/bingemap.component';
import { AgmCoreModule } from '@agm/core';
import { ServiceWorkerModule } from '@angular/service-worker';
import { environment } from '../environments/environment';
import { EcoFabSpeedDialModule } from '@ecodev/fab-speed-dial';
import { ActivityComponent } from './components/activity/activity.component';
import { FormsModule } from '@angular/forms';
import { ProgressSpinnerComponent } from './components/progress-spinner/progress-spinner.component';
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
import { DrinkIconComponent } from './components/drink-icon/drink-icon.component';


// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    NavShellComponent,
    WelcomeComponent,
    ActivityFeedComponent,
    MeComponent,
    StatsComponent,
    UserInfoComponent,
    CallbackComponent,
    BingemapComponent,
    ActivityComponent,
    ProgressSpinnerComponent,
    DrinkChartComponent,
    DrinkRatioChartComponent,
    ProfileComponent,
    FriendrequestsComponent,
    DrinkersComponent,
    DrinkDialogComponent,
    NoFriendsComponent,
    MessageDialogComponent,
    DrinkIconComponent
  ],
  entryComponents: [DrinkDialogComponent, MessageDialogComponent],
  imports: [
    BrowserModule,
    InViewportModule,
    FormsModule,
    HttpClientModule,
    ChartsModule,
    EcoFabSpeedDialModule,
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyBlGceFBW7ykKMzNH4o0DwMBlxwt8NgWc8'
    }),
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    AppRoutingModule,
    BrowserAnimationsModule,
    LayoutModule,
    FlexLayoutModule,
    AppMaterialModule,
    ServiceWorkerModule.register('ngsw-worker.js', { enabled: environment.production }),
    FileUploadModule
  ],
  providers: [
    DataService, AuthService, UserService, ActivityService, NotificationService,
    FriendRequestService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    }],
  bootstrap: [AppComponent]
})
export class AppModule { }
