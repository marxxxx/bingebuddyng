import { DrinkType } from './DrinkType';

export class DrinkDTO {
  id?: string;
  drinkType: DrinkType;
  name: string;
  alcPrc: number;
  volume: number;
}
