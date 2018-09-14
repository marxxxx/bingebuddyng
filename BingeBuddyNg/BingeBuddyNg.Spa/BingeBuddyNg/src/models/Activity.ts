import { DrinkType } from './DrinkType';
import { LocationDTO } from './LocationDTO';
import { ActivityType } from './ActivityType';
import { Reaction } from './Reaction';
import { CommentReaction } from './CommentReaction';
export class Activity {
    id: string;
    activityType: ActivityType;
    timestamp: Date;
    userId: string;
    userProfileImageUrl: string;
    userName: string;
    message: string;
    locationAddress: string;
    location: LocationDTO;
    drinkType: DrinkType;
    drinkName: string;
    imageUrl: string;
    likes: Reaction[];
    cheers: Reaction[];
    comments: CommentReaction[];
}
