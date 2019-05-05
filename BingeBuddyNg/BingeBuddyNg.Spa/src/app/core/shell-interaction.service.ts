import { MatSidenav, MatSnackBar, MatDialog } from '@angular/material';
import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ConfirmationDialogComponent } from '../shared/confirmation-dialog/confirmation-dialog.component';
import { ConfirmationDialogArgs } from '../shared/confirmation-dialog/ConfirmationDialogArgs';
import { Observable } from 'rxjs';

@Injectable()
export class ShellInteractionService {

  private sideNav: MatSidenav;


  constructor(private translate: TranslateService, private snackbar: MatSnackBar,
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
    const message = this.translate.instant('OperationFailed');
    this.snackbar.open(message, 'OK');
  }

  showMessage(message: string) {
    const translatedMessage = this.translate.instant(message);
    this.snackbar.open(translatedMessage, 'OK');
  }

  showConfirmationDialog(args: ConfirmationDialogArgs): Observable<any> {
    return this.dialog.open(ConfirmationDialogComponent, {data: args}).afterClosed();
  }
}
