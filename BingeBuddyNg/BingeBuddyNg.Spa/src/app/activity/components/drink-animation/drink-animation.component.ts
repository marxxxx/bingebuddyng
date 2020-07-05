import { DrinkType } from 'src/models/DrinkType';
import { Component, OnInit, Input } from '@angular/core';
import { beerGifs, wineGifs, shotGifs, antiGifs } from './gifs';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-drink-animation',
  templateUrl: './drink-animation.component.html',
  styleUrls: ['./drink-animation.component.css']
})
export class DrinkAnimationComponent implements OnInit {

  readonly gifCount = {
    beer: 59,
    wine: 11,
    shot: 11,
    anti: 4
  };

  @Input() drinkType: DrinkType;

  imageUrl: string;
  fallbackImageUrl: string;

  constructor() { }

  ngOnInit(): void {
    this.imageUrl = this.getRandomDrinkImageUrl('.webp');
    this.fallbackImageUrl = this.getRandomDrinkImageUrl('.gif');

    const audio = new Audio('/assets/sound/bierflasche.wav');
    audio.play();
  }

  getRandomDrinkImageUrl(extension: string): string {
    let url = environment.drinkImgBaseUrl;
    switch (this.drinkType) {
      case DrinkType.Beer: {
        url += 'beer/' + this.getRandomNumber(this.gifCount.beer) + extension;
        break;
      }
      case DrinkType.Wine: {
        url += 'wine/' + this.getRandomNumber(this.gifCount.wine) + extension;
        break;
      }
      case DrinkType.Shot: {
        url += 'shot/' + this.getRandomNumber(this.gifCount.shot) + extension;
        break;
      }
      case DrinkType.Anti: {
        url += 'anti/' + this.getRandomNumber(this.gifCount.anti) + extension;
        break;
      }
      default:
        url = null;
        break;
    }
    return url;
  }

  getRandomNumber(max: number): number {
    const randomNumber = Math.floor(Math.random() * (max));
    return randomNumber;
  }
}
