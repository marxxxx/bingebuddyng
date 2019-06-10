import { OnboardingComponent } from './onboarding/components/onboarding/onboarding.component';

import { CallbackComponent } from './core/components/callback/callback.component';
import { ActivityFeedComponent } from './activity/components/activity-feed/activity-feed.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './core/services/auth.guard';
import { SettingsComponent } from './core/components/settings/settings.component';
import { WelcomeComponent } from './onboarding/components/welcome/welcome.component';

const routes: Routes = [
  {
    path: 'callback',
    component: CallbackComponent
  },
  {
    path: 'settings',
    component: SettingsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'welcome',
    component: WelcomeComponent
  },
  {
    path: 'onboarding',
    component: OnboardingComponent
  },
  {
    path: 'activity-feed',
    component: ActivityFeedComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'stats',
    loadChildren: () => import('src/app/statistics/statistics.module').then(m => m.StatisticsModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'bingemap',
    loadChildren: () => import('src/app/drinkmap/drink-map.module').then(m => m.DrinkMapModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'invitation',
    loadChildren: () => import('src/app/invitation/invitation.module').then(m => m.InvitationModule)
  },
  {
    path: 'ranking',
    loadChildren: () => import('src/app/ranking/ranking.module').then(m => m.RankingModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'drinks',
    loadChildren: () => import('src/app/drinks/drinks.module').then(m => m.DrinksModule),
    canActivate: [AuthGuard]
  },
  {
    path: 'users',
    loadChildren: () => import('src/app/users/users.module').then(m => m.UsersModule),
    canActivate: [AuthGuard]
  },
  {
    path: '',
    component: WelcomeComponent
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
