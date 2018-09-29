import { DrinkersComponent } from './pages/drinkers/drinkers.component';
import { FriendrequestsComponent } from './pages/friendrequests/friendrequests.component';
import { ProfileComponent } from './pages/profile/profile.component';
import { BingemapComponent } from './pages/bingemap/bingemap.component';
import { CallbackComponent } from './components/callback/callback.component';
import { StatsComponent } from './pages/stats/stats.component';
import { ActivityFeedComponent } from './pages/activity-feed/activity-feed.component';
import { WelcomeComponent } from './pages/welcome/welcome.component';
import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

const routes: Routes = [{
  path: 'callback',
  component: CallbackComponent
}, {
  path: 'welcome',
  component: WelcomeComponent
},
{
  path: 'activity-feed',
  component: ActivityFeedComponent
}, {
  path: 'stats',
  component: StatsComponent
},
{
  path: 'bingemap',
  component: BingemapComponent
}, {
  path: 'profile/:userId',
  component: ProfileComponent
}, {
  path: 'friendrequests',
  component: FriendrequestsComponent
}, {
  path: 'drinkers',
  component: DrinkersComponent
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
