import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { ProfileComponent } from './components/profile/profile.component';
import { DrinkersComponent } from './components/drinkers/drinkers.component';
import { AuthGuard } from '../@core/services/auth.guard';
import { SharedModule } from '../@shared/shared.module';
import { FriendrequestsComponent } from './components/friendrequests/friendrequests.component';
import { ProfileImageDialogComponent } from './components/profile-image-dialog/profile-image-dialog.component';

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
  entryComponents: [ProfileImageDialogComponent],
  declarations: [ProfileComponent, DrinkersComponent, FriendrequestsComponent, ProfileImageDialogComponent]
})
export class UsersModule {}
