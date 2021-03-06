import { Component, OnInit, Output, EventEmitter, OnDestroy } from '@angular/core';
import { DrinkType } from 'src/models/DrinkType';

@Component({
  selector: 'app-game-pad',
  templateUrl: './game-pad.component.html',
  styleUrls: ['./game-pad.component.scss']
})
export class GamePadComponent implements OnInit, OnDestroy {

  drinks: DrinkType[] = [DrinkType.Unknown, DrinkType.Unknown, DrinkType.Unknown, DrinkType.Unknown, DrinkType.Unknown];
  timer: any;
  DrinkTypeEnum = DrinkType;

  @Output()
  scored = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
    this.timer = setInterval(() => {
      this.generateRandomDrinkTypes();
    }, 1000);
  }

  ngOnDestroy() {
    if (this.timer) {
      clearInterval(this.timer);
      this.timer = null;
    }
  }

  onDrink(drinkType: DrinkType) {
    if (drinkType !== DrinkType.Anti) {
      this.scored.emit(drinkType);
    }

    for (let i = 0; i < this.drinks.length; i++) {
      this.drinks[i] = DrinkType.Unknown;
    }
  }

  generateRandomDrinkTypes() {
    const newDrinks = [];

    for (let i = 0; i < this.drinks.length; i++) {
      newDrinks[i] = DrinkType[this.getRandomDrinkType()];
    }
    this.drinks = newDrinks;
    console.log(this.drinks);
  }

  getRandomDrinkType(): DrinkType {
    const index = this.getRandomNumber(5);
    const drinkType = DrinkType[Object.keys(DrinkType)[index]];
    return drinkType;
  }

  getRandomNumber(max: number): number {
    const randomIndex = Math.floor(Math.random() * max);
    return randomIndex;
  }
}
