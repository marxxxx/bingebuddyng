
import { CallbackComponent } from './core/callback/callback.component';
import { ActivityFeedComponent } from './activity/activity-feed/activity-feed.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from './core/auth.guard';
import { SettingsComponent } from './core/settings/settings.component';
import { WelcomeComponent } from './onboarding/welcome/welcome.component';

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
    path: 'activity-feed',
    component: ActivityFeedComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'stats',
    loadChildren: 'src/app/statistics/statistics.module#StatisticsModule',
    canActivate: [AuthGuard]
  },
  {
    path: 'bingemap',
    loadChildren: 'src/app/drinkmap/drink-map.module#DrinkMapModule',
    canActivate: [AuthGuard]
  },
  {
    path: 'invitation',
    loadChildren: 'src/app/invitation/invitation.module#InvitationModule',
  },
  {
    path: 'ranking',
    loadChildren: 'src/app/ranking/ranking.module#RankingModule',
  },
  {
    path: 'drinks',
    loadChildren: 'src/app/drinks/drinks.module#DrinksModule'
  },
  {
    path: 'users',
    loadChildren: 'src/app/users/users.module#UsersModule'
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
export class AppRoutingModule {}
