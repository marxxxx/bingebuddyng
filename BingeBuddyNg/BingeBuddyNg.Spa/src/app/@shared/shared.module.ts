import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppMaterialModule } from './app-material.module';
import { ProgressSpinnerComponent } from './components/progress-spinner/progress-spinner.component';
import { UserInfoComponent } from './components/user-info/user-info.component';
import { FlexLayoutModule } from '@angular/flex-layout';
import { TranslocoModule } from '@ngneat/transloco';
import { DrinkIconComponent } from './components/drink-icon/drink-icon.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ConfirmationDialogComponent } from './components/confirmation-dialog/confirmation-dialog.component';
import { FileUploadModule } from 'ng2-file-upload';
import { RouterModule } from '@angular/router';



@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    AppMaterialModule,
    FlexLayoutModule,
    TranslocoModule,
    FileUploadModule,
    RouterModule
  ],
  exports: [
    CommonModule,
    AppMaterialModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    TranslocoModule,
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
  entryComponents: [ConfirmationDialogComponent],
  providers: []
})
export class SharedModule { }
