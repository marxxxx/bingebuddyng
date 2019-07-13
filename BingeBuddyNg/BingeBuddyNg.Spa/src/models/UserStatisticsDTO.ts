export class UserStatisticsDTO {
    userId: string;
    currentAlcoholization: number;
    currentNightDrinks: number;
    totalDrinksLastMonth?: number;
    score?: number;
}
