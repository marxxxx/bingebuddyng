import { NgModule } from '@angular/core';
import { SharedModule } from '../@shared/shared.module';
import { PlayfieldComponent } from './components/playfield/playfield.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../@core/services/auth.guard';
import { EndGameComponent } from './components/end/end-game.component';
import { StartGameComponent } from './components/start/start-game.component';
import { GamePadComponent } from './components/game-pad/game-pad.component';

const routes: Routes = [
  {
    path: '',
    component: StartGameComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'play/:gameId',
    component: PlayfieldComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'end/:gameId',
    component: EndGameComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  declarations: [StartGameComponent, PlayfieldComponent, EndGameComponent, GamePadComponent],
  imports: [
    SharedModule,
    RouterModule.forChild(routes)
  ]
})
export class GameModule { }
