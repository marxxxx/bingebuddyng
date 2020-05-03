import { NgModule } from '@angular/core';
import { SharedModule } from '../@shared/shared.module';
import { PlayfieldComponent } from './components/playfield/playfield.component';
import { SelectPlayersComponent } from './components/select-players/select-players.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../@core/services/auth.guard';

const routes: Routes = [
  {
    path: '',
    component: SelectPlayersComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'play/:gameId',
    component: PlayfieldComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  declarations: [SelectPlayersComponent, PlayfieldComponent],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class GameModule { }
