import { CallbackComponent } from './components/callback/callback.component';
import { MeComponent } from './pages/me/me.component';
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
  path: 'me',
  component: MeComponent
}, {
  path: 'stats',
  component: StatsComponent
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
