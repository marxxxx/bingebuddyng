import { NgModule } from '@angular/core';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { HTTP_INTERCEPTORS, HttpClient } from '@angular/common/http';
import { AuthHttpInterceptor } from './services/auth.interceptor';
import { CallbackComponent } from './components/callback/callback.component';
import { DrinkEventCounterComponent } from './components/drink-event-counter/drink-event-counter.component';
import { MeComponent } from './components/me/me.component';
import { NavShellComponent } from './components/nav-shell/nav-shell.component';
import { SharedModule } from '../@shared/shared.module';
import { RouterModule } from '@angular/router';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { SettingsComponent } from './components/settings/settings.component';


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
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    }
  ]
})
export class CoreModule {}
