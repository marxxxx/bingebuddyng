
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { MatButtonModule, MatCheckboxModule, MatAutocompleteModule, MatCardModule, MatPaginatorModule} from '@angular/material';
import { MatDatepickerModule, MatDialogModule, MatInputModule, MatListModule } from '@angular/material';
import { MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule } from '@angular/material';
import { MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatTabsModule } from '@angular/material';
import { MatToolbarModule, MatTooltipModule, MatIconModule, MatExpansionModule, MatTableModule } from '@angular/material';
import { MatNativeDateModule, MatStepperModule, MatChipsModule } from '@angular/material';


@NgModule({
  imports: [
    CommonModule,
    MatButtonModule, MatCheckboxModule,
    MatAutocompleteModule, MatCardModule, MatDatepickerModule, MatDialogModule,
    MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule,
    MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatTabsModule, MatToolbarModule,
    MatTooltipModule, MatIconModule, MatExpansionModule, MatTableModule, MatNativeDateModule, MatStepperModule,
    MatPaginatorModule, MatChipsModule
  ],
  exports: [
    MatButtonModule, MatCheckboxModule,
    MatAutocompleteModule, MatCardModule, MatDatepickerModule, MatDialogModule,
    MatInputModule, MatListModule, MatMenuModule, MatProgressBarModule, MatProgressSpinnerModule, MatRadioModule,
    MatSelectModule, MatSidenavModule, MatSlideToggleModule, MatSnackBarModule, MatTabsModule, MatToolbarModule,
    MatTooltipModule, MatIconModule, MatExpansionModule, MatTableModule, MatNativeDateModule, MatStepperModule,
    MatPaginatorModule, MatChipsModule
  ]
})
export class AppMaterialModule { }
