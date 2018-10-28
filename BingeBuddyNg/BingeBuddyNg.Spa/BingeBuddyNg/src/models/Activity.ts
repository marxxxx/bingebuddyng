import { DrinkType } from './DrinkType';
import { LocationDTO } from './LocationDTO';
import { ActivityType } from './ActivityType';
import { Reaction } from './Reaction';
import { CommentReaction } from './CommentReaction';
import { UserInfo } from './UserInfo';
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

    getUserInfo(): UserInfo {
        const userInfo: UserInfo = {
            userId: this.userId,
            userName: this.userName,
            userProfileImageUrl: this.userProfileImageUrl
        };

        return userInfo;
    }
}
