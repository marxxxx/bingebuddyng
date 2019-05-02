import { VenueModel } from 'src/models/VenueModel';
import { DrinkType } from './DrinkType';
import { Location } from './Location';
import { ActivityType } from './ActivityType';
import { Reaction } from './Reaction';
import { CommentReaction } from './CommentReaction';
import { UserInfo } from './UserInfo';
export class Activity {
    id: string;
    activityType: ActivityType;
    timestamp: Date;
    userId: string;
    userName: string;
    message: string;
    locationAddress: string;
    location: Location;
    drinkType: DrinkType;
    drinkName: string;
    drinkCount: number;
    imageUrl: string;
    venue?: VenueModel;
    likes: Reaction[];
    cheers: Reaction[];
    comments: CommentReaction[];
    registrationUser?: UserInfo;

    getUserInfo(): UserInfo {
        const userInfo: UserInfo = {
            userId: this.userId,
            userName: this.userName
        };

        return userInfo;
    }
}
