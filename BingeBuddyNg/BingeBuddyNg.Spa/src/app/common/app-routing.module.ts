import { WelcomeInvitedComponent } from '../pages/welcome-invited/welcome-invited.component';
import { DrinkersComponent } from '../pages/drinkers/drinkers.component';
import { FriendrequestsComponent } from '../pages/friendrequests/friendrequests.component';
import { ProfileComponent } from '../pages/profile/profile.component';
import { BingemapComponent } from '../pages/bingemap/bingemap.component';
import { CallbackComponent } from '../components/callback/callback.component';
import { StatsComponent } from '../pages/stats/stats.component';
import { ActivityFeedComponent } from '../pages/activity-feed/activity-feed.component';
import { WelcomeComponent } from '../pages/welcome/welcome.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../core/auth.guard';
import { RankingComponent } from '../pages/ranking/ranking.component';
import { InviteFriendComponent } from '../pages/invite-friend/invite-friend.component';
import { SettingsComponent } from '../pages/settings/settings.component';
import { DrinksComponent } from '../drinks/drinks.component';
import { AddOrEditDrinkComponent } from '../drinks/add-or-edit-drink/add-or-edit-drink.component';
import { OnboardingComponent } from '../pages/onboarding/onboarding.component';

const routes: Routes = [
  {
    path: 'callback',
    component: CallbackComponent
  },
  {
    path: 'welcome',
    component: WelcomeComponent
  },
  {
    path: 'welcome-invited/:invitationToken',
    component: WelcomeInvitedComponent
  },
  {
    path: 'activity-feed',
    component: ActivityFeedComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'stats',
    component: StatsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'bingemap',
    component: BingemapComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'profile/:userId',
    component: ProfileComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'friendrequests',
    component: FriendrequestsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'drinkers',
    component: DrinkersComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'ranking',
    component: RankingComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'settings',
    component: SettingsComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'invite-friend',
    component: InviteFriendComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'onboarding',
    component: OnboardingComponent
  },
  {
    path: 'drinks',
    loadChildren: 'src/app/drinks/drinks.module#DrinksModule'
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
