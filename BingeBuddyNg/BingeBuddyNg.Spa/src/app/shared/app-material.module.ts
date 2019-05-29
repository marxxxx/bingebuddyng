
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatBadgeModule } from '@angular/material/badge';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTabsModule } from '@angular/material/tabs';
import { MatDialogModule } from '@angular/material/dialog';
import { MatInputModule } from '@angular/material/input';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatRadioModule } from '@angular/material/radio';
import { MatSelectModule } from '@angular/material/select';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatChipsModule } from '@angular/material/chips';




@NgModule({
  imports: [
    CommonModule,
    MatButtonModule, MatCheckboxModule,
    MatCardModule, MatDialogModule,
    MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule,
    MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatToolbarModule,
    MatTooltipModule, MatIconModule, MatChipsModule, MatTabsModule,
    MatBadgeModule
  ],
  exports: [
    MatButtonModule, MatCheckboxModule,
    MatCardModule, MatDialogModule,
    MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule,
    MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatToolbarModule,
    MatTooltipModule, MatIconModule, MatChipsModule, MatTabsModule,
    MatBadgeModule
  ]
})
export class AppMaterialModule { }
