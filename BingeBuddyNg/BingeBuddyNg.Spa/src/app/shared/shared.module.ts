import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from './app-material.module';
import { ProgressSpinnerComponent } from './progress-spinner/progress-spinner.component';
import { UserInfoComponent } from './user-info/user-info.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TranslateModule } from '@ngx-translate/core';
import { DrinkIconComponent } from './drink-icon/drink-icon.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationDialogComponent } from './confirmation-dialog/confirmation-dialog.component';
import { FileUploadModule } from 'ng2-file-upload';
import { RouterModule } from '@angular/router';



@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule,
    FlexLayoutModule,
    TranslateModule.forChild(),
    FileUploadModule,
    RouterModule
  ],
  exports: [
    CommonModule,
    AppMaterialModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    FileUploadModule,
    ProgressSpinnerComponent,
    UserInfoComponent,
    DrinkIconComponent,
    ConfirmationDialogComponent
  ],
  declarations: [
    ProgressSpinnerComponent,
    UserInfoComponent,
    DrinkIconComponent,
    ConfirmationDialogComponent
  ],
  providers: []
})
export class SharedModule {}
