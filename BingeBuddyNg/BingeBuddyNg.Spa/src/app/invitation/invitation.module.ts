import { NgModule } from '@angular/core';
import { SharedModule } from '../@shared/shared.module';
import { InviteFriendComponent } from './components/invite-friend/invite-friend.component';
import { Routes, RouterModule } from '@angular/router';
import { WelcomeInvitedComponent } from './components/welcome-invited/welcome-invited.component';
import { AuthGuard } from '../@core/services/auth.guard';

const routes: Routes = [
  {
    path: 'invite-friend',
    component: InviteFriendComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'welcome/:invitationToken',
    component: WelcomeInvitedComponent
  }
];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(routes)],
  exports: [],
  declarations: [InviteFriendComponent,  WelcomeInvitedComponent],
  providers: [],
})
export class InvitationModule { }
