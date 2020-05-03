import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { GameService } from '../../services/game.service';

@Component({
  selector: 'app-playfield',
  templateUrl: './playfield.component.html',
  styleUrls: ['./playfield.component.css']
})
export class PlayfieldComponent implements OnInit {

  constructor(private route: ActivatedRoute, private gameService: GameService) { }

  ngOnInit(): void {
    this.route.paramMap.subscribe(p => {
      const gameId = p.get('gameId');
      console.log('starting game', gameId);
    });
  }

}
