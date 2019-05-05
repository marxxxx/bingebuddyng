import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { SettingsComponent } from '../core/settings/settings.component';
import { DrinkersComponent } from './drinkers/drinkers.component';
import { AuthGuard } from '../core/auth.guard';
import { SharedModule } from '../shared/shared.module';
import { FriendrequestsComponent } from './friendrequests/friendrequests.component';

const routes: Routes = [
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
    path: '',
    pathMatch: 'full',
    component: DrinkersComponent,
    canActivate: [AuthGuard]
  },
];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(routes)],
  exports: [],
  declarations: [ProfileComponent, DrinkersComponent, FriendrequestsComponent]
})
export class UsersModule {}
