import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from '../common/app-material.module';
import { ProgressSpinnerComponent } from './progress-spinner/progress-spinner.component';
import { UserInfoComponent } from './user-info/user-info.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { DrinkIconComponent } from './drink-icon/drink-icon.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

// AoT requires an exported function for factories
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http);
}

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule,
    FlexLayoutModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })
  ],
  exports: [
    CommonModule,
    AppMaterialModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    ProgressSpinnerComponent,
    UserInfoComponent,
    DrinkIconComponent,
    TranslateModule
  ],
  declarations: [
    ProgressSpinnerComponent,
    UserInfoComponent,
    DrinkIconComponent
  ],
  providers: []
})
export class SharedModule {}
