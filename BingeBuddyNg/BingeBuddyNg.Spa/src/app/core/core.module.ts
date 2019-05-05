import { NgModule } from '@angular/core';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { AuthService } from './auth.service';
import { AuthGuard } from './auth.guard';
import { AuthHttpInterceptor } from './auth.interceptor';
import { DrinkEventService } from './drinkevent.service';
import { SettingsService } from './settings.service';
import { StateService } from './state.service';
import { NotificationService } from './notification.service';
import { CallbackComponent } from './callback/callback.component';
import { DrinkEventCounterComponent } from './drink-event-counter/drink-event-counter.component';
import { MeComponent } from './me/me.component';
import { NavShellComponent } from './nav-shell/nav-shell.component';
import { ShellInteractionService } from './shell-interaction.service';
import { SharedModule } from '../shared/shared.module';
import { RouterModule } from '@angular/router';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { LocationService } from './location.service';
import { DrinkActivityService } from './drink-activity.service';
import { SettingsComponent } from './settings/settings.component';
import { FriendRequestService } from './friendrequest.service';
import { UserService } from './user.service';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

const translateArgs = {
  loader: {
    provide: TranslateLoader,
    useFactory: HttpLoaderFactory,
    deps: [HttpClient]
  }
};

@NgModule({
  imports: [SharedModule, TranslateModule.forRoot(translateArgs), RouterModule],
  exports: [NavShellComponent],
  declarations: [CallbackComponent, DrinkEventCounterComponent, MeComponent, NavShellComponent, SettingsComponent],
  providers: [
    AuthService,
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    },
    DrinkEventService,
    SettingsService,
    StateService,
    NotificationService,
    ShellInteractionService,
    LocationService,
    DrinkActivityService,
    FriendRequestService,
    UserService,
    NotificationService
  ]
})
export class CoreModule {}
