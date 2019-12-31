import { MatDialog } from '@angular/material/dialog';
import { MatSidenav } from '@angular/material/sidenav';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Injectable } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ConfirmationDialogComponent } from '../../@shared/components/confirmation-dialog/confirmation-dialog.component';
import { ConfirmationDialogArgs } from '../../@shared/components/confirmation-dialog/ConfirmationDialogArgs';
import { Observable } from 'rxjs';

@Injectable({providedIn: 'root'})
export class ShellInteractionService {

  private sideNav: MatSidenav;


  constructor(private translate: TranslocoService, private snackbar: MatSnackBar,
    private dialog: MatDialog) { }

  registerSideNav(sideNav: MatSidenav) {
    this.sideNav = sideNav;
  }

  setSideNavState(open: boolean) {
    if (open) {
      this.sideNav.open();
    } else {
      this.sideNav.close();
    }
  }

  showErrorMessage() {
    const message = this.translate.translate('OperationFailed');
    this.snackbar.open(message, 'OK');
  }

  showMessage(message: string) {
    const translatedMessage = this.translate.translate(message);
    this.snackbar.open(translatedMessage, 'OK');
  }

  showConfirmationDialog(args: ConfirmationDialogArgs): Observable<any> {
    return this.dialog.open(ConfirmationDialogComponent, {data: args}).afterClosed();
  }
}
