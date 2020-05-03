import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { GameService } from '../../services/game.service';
import { GameDTO } from '../../models/GameDTO';
import { ShellInteractionService } from 'src/app/@core/services/shell-interaction.service';
import { finalize } from 'rxjs/operators';
import { NotificationService } from 'src/app/@core/services/notification.service';
import { GameUpdateReceivedMessage } from '../../models/GameUpdateReceivedMessage';
import { GameEndedMessage } from '../../models/GameEndedMessage';
import { GameStatus } from '../../models/GameStatus';

@Component({
  selector: 'app-playfield',
  templateUrl: './playfield.component.html',
  styleUrls: ['./playfield.component.css']
})
export class PlayfieldComponent implements OnInit {

  game: GameDTO;
  isBusy = false;
  GameStatus = GameStatus;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gameService: GameService,
    private shellInteraction: ShellInteractionService,
    private notificationService: NotificationService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(p => {
      const gameId = p.get('gameId');

      this.isBusy = true;

      this.gameService.getGame(gameId)
        .pipe(finalize(() => this.isBusy = false))
        .subscribe(g => this.game = g, e => {
          this.router.navigateByUrl('/game');
        });
    });

    this.notificationService.on('GameUpdateReceived', (payload: GameUpdateReceivedMessage) => {
      console.log('GameUpdateReceived', payload);
      if (payload.gameId !== this.game.id) {
        return;
      }

      const user = this.game.userScores.find(u => u.user.userId === payload.userId);
      user.score = payload.currentScore;
    });

    this.notificationService.on('GameEnded', (payload: GameEndedMessage) => {
      this.shellInteraction.showMessage('Game over!');
      this.game.status = GameStatus.Ended;
      this.game.winnerUserId = payload.winnerUserId;
    });
  }

  onScored(count: number) {
    this.gameService.addGameEvent(this.game.id, { count: count }).subscribe();
  }
}
