import { AddActivityBaseDTO } from './AddActivityBaseDTO';
import { DrinkType } from './DrinkType';

export class AddDrinkActivityDTO extends AddActivityBaseDTO {

    drinkId: string;

    drinkType: DrinkType;

    drinkName: string;

    alcPrc: number;

    volume: number;
}
