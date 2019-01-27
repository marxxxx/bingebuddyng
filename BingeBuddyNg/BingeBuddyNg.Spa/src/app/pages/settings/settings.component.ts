import { User } from './../../../models/User';
import { AuthService } from './../../services/auth.service';
import { UserService } from 'src/app/services/user.service';
import { SettingsService } from './../../services/settings.service';
import { ShellInteractionService } from './../../services/shell-interaction.service';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { ActivatedRoute } from '@angular/router';
import { Subscription } from 'rxjs';

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
  public languages = [{ lang: 'de', text: 'Deutsch' }, { lang: 'en', text: 'English' }];
  public currentLanguage = this.settingsService.getLanguage();
  subscriptions: Subscription[] = [];
  currentUser: User;

  constructor(private translateService: TranslateService,
    private settingsService: SettingsService,
    private shellInteraction: ShellInteractionService,
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute) {
  }


  ///////////////////////////////////////////////////////
  // functions
  ///////////////////////////////////////////////////////
  ngOnInit() {
    const sub = this.route.params.subscribe(r => {
      this.currentLanguage = this.translateService.currentLang;

      const langChangeSubscription = this.translateService.onLangChange.subscribe(l => {
        this.currentLanguage = l.lang;
      });
      this.subscriptions.push(langChangeSubscription);
    });
    this.subscriptions.push(sub);


    this.subscriptions.push(this.authService.currentUserProfile$.subscribe(p => {
      if (p != null) {
        this.userService.getUser(p.sub).subscribe(u => this.currentUser = u);
      }
    }));

  }


  onLanguageChanged(language) {
    if (language) {
      console.log('SettingsComponent: language set to ' + language.value);
      this.translateService.use(language.value);
      this.settingsService.setLanguage(language.value);
      if (this.currentUser != null) {
        this.currentUser.language = language.value;
        this.userService.saveUser(this.currentUser).subscribe(_ => console.log('user saved', this.currentUser));
      }
    } else {
      console.log('SettingsComponent: language unchanged');
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(s => s.unsubscribe());
  }
}


