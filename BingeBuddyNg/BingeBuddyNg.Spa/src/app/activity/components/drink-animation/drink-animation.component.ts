import { DrinkType } from 'src/models/DrinkType';
import { Component, OnInit, Input } from '@angular/core';
import { beerGifs, wineGifs, shotGifs, antiGifs } from './gifs';

@Component({
  selector: 'app-drink-animation',
  templateUrl: './drink-animation.component.html',
  styleUrls: ['./drink-animation.component.css']
})
export class DrinkAnimationComponent implements OnInit {

  @Input() drinkType: DrinkType;

  imageUrl: string;

  constructor() { }

  ngOnInit(): void {
    this.imageUrl = this.getRandomDrinkImageUrl();

    const audio = new Audio('/assets/sound/bierflasche.wav');
    audio.play();
  }

  getRandomDrinkImageUrl(): string {
    let gifs: string[] = null;
    switch (this.drinkType) {
      case DrinkType.Beer: {
        gifs = beerGifs;
        break;
      }
      case DrinkType.Wine: {
        gifs = wineGifs;
        break;
      }
      case DrinkType.Shot: {
        gifs = shotGifs;
        break;
      }
      case DrinkType.Anti: {
        gifs = antiGifs;
        break;
      }
      default:
        return '/assets/img/funny_beers.gif';
    }

    return this.getRandomLink(gifs);
  }

  getRandomLink(gifs: string[]): string {
    const index = Math.floor(Math.random() * (gifs.length - 1));
    return gifs[index];
  }
}
