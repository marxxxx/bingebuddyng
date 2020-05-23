import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GameService } from '../../services/game.service';
import { GameDTO } from '../../models/GameDTO';
import { ShellInteractionService } from 'src/app/@core/services/shell-interaction.service';
import { finalize } from 'rxjs/operators';

@Component({
  selector: 'app-end-game',
  templateUrl: './end-game.component.html',
  styleUrls: ['./end-game.component.scss']
})
export class EndGameComponent implements OnInit {

  game: GameDTO;
  isBusy = false;

  constructor(
    private route: ActivatedRoute,
    private gameService: GameService,
    private shellInteraction: ShellInteractionService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(p => {
      const gameId = p.get('gameId');
      this.load(gameId);
    });
  }

  load(gameId: string): void {
    this.isBusy = true;
    this.gameService.getGame(gameId)
      .pipe(finalize(() => this.isBusy = false))
      .subscribe(g => {
        this.game = g;
      }, e => {
        this.shellInteraction.showErrorMessage();
      });
  }
}
