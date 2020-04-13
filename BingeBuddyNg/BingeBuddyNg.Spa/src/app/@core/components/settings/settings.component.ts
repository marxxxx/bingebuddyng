import { CreateOrUpdateUserDTO } from './../../../../models/CreateOrUpdateUserDTO';
import { UserDTO } from '../../../../models/UserDTO';
import { AuthService } from '../../services/auth/auth.service';
import { UserService } from 'src/app/@core/services/user.service';
import { SettingsService } from '../../services/settings.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslocoService } from '@ngneat/transloco';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';

/**
 * User Settings Page.
 */
@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit, OnDestroy {
  ///////////////////////////////////////////////////////
  // members
  ///////////////////////////////////////////////////////
  public isBusy = false;
  public languages = [
    { lang: 'at', text: 'Österreichisch' },
    { lang: 'de', text: 'Deutsch' },
    { lang: 'en', text: 'English' },
    { lang: 'ru', text: 'русский' }
  ];
  public currentLanguage = this.settingsService.getLanguage();
  subscriptions: Subscription[] = [];
  currentUser: UserDTO;

  constructor(
    private translateService: TranslocoService,
    private settingsService: SettingsService,
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute
  ) { }

  ///////////////////////////////////////////////////////
  // functions
  ///////////////////////////////////////////////////////
  ngOnInit() {
    const sub = this.route.params.subscribe(r => {
      this.currentLanguage = this.translateService.getActiveLang();

      const langChangeSubscription = this.translateService.langChanges$.subscribe(l => {
        this.currentLanguage = l;
      });
      this.subscriptions.push(langChangeSubscription);
    });
    this.subscriptions.push(sub);

    this.subscriptions.push(
      this.authService.currentUserProfile$
        .pipe(filter(p => p != null))
        .subscribe(p => {
          this.userService.getUser(p.sub).subscribe(u => (this.currentUser = u));
        })
    );
  }

  onLanguageChanged(language) {
    if (language) {
      console.log('SettingsComponent: language set to ' + language.value);
      this.translateService.setActiveLang(language.value);
      this.settingsService.setLanguage(language.value);
      if (this.currentUser != null) {
        this.currentUser.language = language.value;

        const request = new CreateOrUpdateUserDTO(
          this.currentUser.id,
          this.currentUser.name,
          this.currentUser.profileImageUrl,
          this.currentUser.pushInfo,
          this.currentUser.language
        );

        this.userService.createOrUpdateUser(request).subscribe(() => {
          console.log('user saved', this.currentUser);
        });
      }
    } else {
      console.log('SettingsComponent: language unchanged');
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }
}
