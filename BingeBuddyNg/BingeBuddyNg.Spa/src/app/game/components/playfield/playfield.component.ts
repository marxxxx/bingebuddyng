import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

import { finalize } from 'rxjs/operators';

import { GameService } from '../../services/game.service';
import { GameDTO } from '../../models/GameDTO';
import { NotificationService } from 'src/app/@core/services/notification.service';
import { GameUpdateReceivedMessage } from '../../models/GameUpdateReceivedMessage';
import { GameEndedMessage } from '../../models/GameEndedMessage';
import { GameStatus } from '../../models/GameStatus';

@Component({
  selector: 'app-playfield',
  templateUrl: './playfield.component.html',
  styleUrls: ['./playfield.component.scss']
})
export class PlayfieldComponent implements OnInit, OnDestroy {

  game: GameDTO;
  isBusy = false;
  GameStatus = GameStatus;
  progressValue: number;
  secondsLeft: number;
  timer: any;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private gameService: GameService,
    private notificationService: NotificationService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(p => {
      const gameId = p.get('gameId');

      this.load(gameId);
    });

    this.notificationService.on('GameUpdateReceived', (payload: GameUpdateReceivedMessage) => {
      this.onGameUpdateReceived(payload);
    });

    this.notificationService.on('GameEnded', (payload: GameEndedMessage) => {
      this.onGameEnded(payload);
    });
  }

  ngOnDestroy() {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }

  onScored(count: number) {
    this.gameService.addGameEvent(this.game.id, { count: count }).subscribe();
  }

  private load(gameId: string) {
    this.isBusy = true;
    this.gameService.getGame(gameId)
      .pipe(finalize(() => this.isBusy = false))
      .subscribe(g => {
        this.game = g;

        if (g.status === GameStatus.Running) {
          this.secondsLeft = 59;

          this.timer = setInterval(() => {
            this.secondsLeft -= 1;
            this.progressValue = this.secondsLeft / 60 * 100;
            if (this.secondsLeft === 0) {
              clearInterval(this.timer);
              this.timer = null;
            }
          }, 1000, 1000);
        }

      }, e => {
        this.router.navigateByUrl('/game');
      });
  }

  private onGameUpdateReceived(payload: GameUpdateReceivedMessage): void {
    if (payload.gameId !== this.game.id) {
      return;
    }

    const user = this.game.userScores.find(u => u.user.userId === payload.userId);
    user.score = payload.currentScore;
  }

  private onGameEnded(payload: GameEndedMessage) {
    this.secondsLeft = 0;
    this.game.status = GameStatus.Ended;
    this.game.winnerUserId = payload.winnerUserId;
  }
}
