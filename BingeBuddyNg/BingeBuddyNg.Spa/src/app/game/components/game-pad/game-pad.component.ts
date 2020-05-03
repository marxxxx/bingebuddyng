import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { GameStatus } from '../../models/GameStatus';

@Component({
  selector: 'app-game-pad',
  templateUrl: './game-pad.component.html',
  styleUrls: ['./game-pad.component.css']
})
export class GamePadComponent implements OnInit {

  score = 0;

  @Output()
  scored = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
  }

  onDrink() {
    this.score++;
    this.scored.emit(1);
  }

}
