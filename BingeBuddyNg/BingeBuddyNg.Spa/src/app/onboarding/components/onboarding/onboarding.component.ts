import { Component, OnInit } from '@angular/core';
import { SettingsService } from 'src/app/core/services/settings.service';

@Component({
  selector: 'app-onboarding',
  templateUrl: './onboarding.component.html',
  styleUrls: ['./onboarding.component.scss']
})
export class OnboardingComponent implements OnInit {

  steps = [
    { label: 'Onboarding_Label_Activitys', description: 'Onboarding_Description_Activitys', image: '/assets/img/onboarding/add_activity.PNG' },
    { label: 'Onboarding_Label_Drinkers', description: 'Onboarding_Description_Drinkers', image: '/assets/img/onboarding/drinkers.PNG' },
    { label: 'Onboarding_Label_Navmenu', description: 'Onboarding_Description_Navmenu', image: '/assets/img/onboarding/navmenu_open.PNG' },
    { label: 'Onboarding_Label_Stats', description: 'Onboarding_Description_Stats', image: '/assets/img/onboarding/stats_today.PNG' },
    { label: 'Onboarding_Label_Map', description: 'Onboarding_Description_Map', image: '/assets/img/onboarding/bingemap.PNG' },
    { label: 'Onboarding_Label_Profile', description: 'Onboarding_Description_Profile', image: '/assets/img/onboarding/my_profile.PNG' },
    { label: 'Onboarding_Label_Drinks', description: 'Onboarding_Description_Drinks', image: '/assets/img/onboarding/custom_drinks.PNG' }
  ];

  constructor(private settings: SettingsService) { }

  ngOnInit() {
    this.settings.setIsOnboarded();
  }

}
