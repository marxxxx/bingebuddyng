import { Component, OnInit } from '@angular/core';
import { SettingsService } from 'src/app/@core/services/settings.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-onboarding',
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.scss']
})
export class OnboardingComponent implements OnInit {

  acceptConditions = false;
  isAlreadyOnboarded = false;

  steps = [
    { label: 'Onboarding.Label_Activitys', description: 'Onboarding.Description_Activitys', image: '/assets/img/onboarding/add_activity.PNG' },
    { label: 'Onboarding.Label_Drinkers', description: 'Onboarding.Description_Drinkers', image: '/assets/img/onboarding/drinkers.PNG' },
    { label: 'Onboarding.Label_Navmenu', description: 'Onboarding.Description_Navmenu', image: '/assets/img/onboarding/navmenu_open.PNG' },
    { label: 'Onboarding.Label_Stats', description: 'Onboarding.Description_Stats', image: '/assets/img/onboarding/stats_today.PNG' },
    { label: 'Onboarding.Label_Map', description: 'Onboarding.Description_Map', image: '/assets/img/onboarding/bingemap.PNG' },
    { label: 'Onboarding.Label_Profile', description: 'Onboarding.Description_Profile', image: '/assets/img/onboarding/my_profile.PNG' },
    { label: 'Onboarding.Label_Drinks', description: 'Onboarding.Description_Drinks', image: '/assets/img/onboarding/custom_drinks.PNG' }
  ];

  constructor(private settings: SettingsService, private router: Router) { }

  ngOnInit() {
    this.isAlreadyOnboarded = this.settings.getIsOnboarded();
  }

  onOnboardingFinished() {
    this.settings.setIsOnboarded();
    this.router.navigateByUrl('/activity-feed');
  }

}
