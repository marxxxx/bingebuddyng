import { NgModule } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthService } from './auth.service';
import { AuthGuard } from './auth.guard';
import { AuthHttpInterceptor } from './auth.interceptor';

@NgModule({
  imports: [TranslateModule.forRoot()],
  exports: [TranslateModule],
  declarations: [],
  providers: [
    AuthService,
    AuthGuard,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthHttpInterceptor,
      multi: true
    }
  ]
})
export class CoreModule {}
