import { DrinkType } from './DrinkType';

export class Drink {
  id?: string;
  drinkType: DrinkType;
  name: string;
  alcPrc: number;
  volume: number;
}
