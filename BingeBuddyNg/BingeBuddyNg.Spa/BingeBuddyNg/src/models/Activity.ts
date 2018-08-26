import { DrinkType } from './DrinkType';
import { LocationDTO } from './LocationDTO';
import { ActivityType } from './ActivityType';
export class Activity {
    id: string;
    activityType: ActivityType;
    timestamp: Date;
    userProfileImageUrl: string;
    userName: string;
    message: string;
    locationAddress: string;
    location: LocationDTO;
    drinkType: DrinkType;
    drinkName: string;
    imageUrl: string;
}
