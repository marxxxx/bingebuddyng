import { Component, OnInit } from '@angular/core';
import { UserInfoDTO } from 'src/models/UserInfoDTO';
import { UserService } from 'src/app/@core/services/user.service';
import { UserDTO } from 'src/models/UserDTO';
import { AuthService } from 'src/app/@core/services/auth/auth.service';
import { filter, finalize } from 'rxjs/operators';
import { MatSelectionListChange } from '@angular/material/list';
import { GameService } from '../../services/game.service';
import { StartGameDTO } from '../../models/StartGameDTO';
import { Router } from '@angular/router';
import { ShellInteractionService } from 'src/app/@core/services/shell-interaction.service';

@Component({
  selector: 'app-select-players',
  templateUrl: './select-players.component.html',
  styleUrls: ['./select-players.component.css']
})
export class SelectPlayersComponent implements OnInit {

  isBusy = false;
  isBusyStartingGame = false;
  gameTitle: string;

  selectedPlayers: string[] = [];

  friends: UserInfoDTO[] = [];

  constructor(
    private auth: AuthService,
    private gameService: GameService,
    private router: Router,
    private shellInteraction: ShellInteractionService) { }

  ngOnInit(): void {
    this.isBusy = true;
    this.auth.currentUserProfile$
      .pipe(filter(p => p && p.user != null))
      .subscribe(p => {
        console.log('got profile', p);
        this.friends = p.user.friends;
        this.isBusy = false;
      });
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
