
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatButtonModule, MatCheckboxModule, MatCardModule, MatGridListModule } from '@angular/material';
import { MatDialogModule, MatInputModule, MatListModule } from '@angular/material';
import { MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule } from '@angular/material';
import { MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule } from '@angular/material';
import { MatToolbarModule, MatTooltipModule, MatIconModule } from '@angular/material';
import { MatChipsModule } from '@angular/material';


@NgModule({
  imports: [
    CommonModule,
    MatButtonModule, MatCheckboxModule,
    MatCardModule, MatDialogModule,
    MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule,
    MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatToolbarModule,
    MatTooltipModule, MatIconModule, MatChipsModule, MatGridListModule
  ],
  exports: [
    MatButtonModule, MatCheckboxModule,
    MatCardModule, MatDialogModule,
    MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule,
    MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatToolbarModule,
    MatTooltipModule, MatIconModule, MatChipsModule, MatGridListModule
  ]
})
export class AppMaterialModule { }
