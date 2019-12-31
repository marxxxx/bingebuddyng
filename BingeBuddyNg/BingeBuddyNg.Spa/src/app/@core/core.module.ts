import { NgModule } from '@angular/core';
import { TranslocoModule } from '@ngneat/transloco';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthHttpInterceptor } from './services/auth.interceptor';
import { CallbackComponent } from './components/callback/callback.component';
import { DrinkEventCounterComponent } from './components/drink-event-counter/drink-event-counter.component';
import { MeComponent } from './components/me/me.component';
import { NavShellComponent } from './components/nav-shell/nav-shell.component';
import { SharedModule } from '../@shared/shared.module';
import { RouterModule } from '@angular/router';
import { SettingsComponent } from './components/settings/settings.component';


@NgModule({
  imports: [SharedModule, TranslocoModule, RouterModule],
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
