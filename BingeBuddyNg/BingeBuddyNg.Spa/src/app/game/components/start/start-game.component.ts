import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { MatSelectionListChange } from '@angular/material/list';
import { filter, finalize } from 'rxjs/operators';

import { UserInfoDTO } from 'src/models/UserInfoDTO';
import { AuthService } from 'src/app/@core/services/auth/auth.service';
import { GameService } from '../../services/game.service';
import { StartGameDTO } from '../../models/StartGameDTO';
import { ShellInteractionService } from 'src/app/@core/services/shell-interaction.service';
import { TranslocoService } from '@ngneat/transloco';

@Component({
  selector: 'app-start-game',
  templateUrl: './start-game.component.html',
  styleUrls: ['./start-game.component.css']
})
export class StartGameComponent implements OnInit {

  isBusy = false;
  isBusyStartingGame = false;
  gameTitle: string;

  selectedPlayers: string[] = [];

  friends: UserInfoDTO[] = [];

  constructor(
    private auth: AuthService,
    private gameService: GameService,
    private router: Router,
    private translate: TranslocoService,
    private shellInteraction: ShellInteractionService) { }

  ngOnInit(): void {
    this.isBusy = true;
    this.auth.currentUserProfile$
      .pipe(filter(p => p && p.user != null))
      .subscribe(p => {
        this.friends = p.user.friends.sort((f1, f2) => f1.userName < f2.userName ? -1 : 1);
        this.isBusy = false;
      });

    this.gameTitle = this.translate.translate('Game.DefaultGameTitle');
  }

  onSelectionChange(change: MatSelectionListChange) {
    if (change.option.selected) {
      this.selectedPlayers.push(change.option.value.userId);
    } else {
      const index = this.selectedPlayers.findIndex(p => p === change.option.value.userId);
      if (index >= 0) {
        this.selectedPlayers.splice(index, 1);
      }
    }
  }

  onStartGame() {
    this.isBusyStartingGame = true;
    const game: StartGameDTO = {
      title: this.gameTitle,
      playerUserIds: this.selectedPlayers
    };
    this.gameService.startGame(game)
      .pipe(finalize(() => this.isBusyStartingGame = false))
      .subscribe(r => {
        this.router.navigate(['/game/play', r.gameId]);
      }, e => {
        this.shellInteraction.showErrorMessage();
      });
  }
}
