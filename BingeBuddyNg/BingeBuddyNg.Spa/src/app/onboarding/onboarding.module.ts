import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { SharedModule } from '../shared/shared.module';
import { WelcomeComponent } from './welcome/welcome.component';
import { OnboardingComponent } from './onboarding/onboarding.component';


@NgModule({
  imports: [SharedModule, RouterModule],
  exports: [],
  declarations: [WelcomeComponent, OnboardingComponent]
})
export class OnboardingModule {}
