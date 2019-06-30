import { VenueModel } from 'src/models/VenueModel';
import { DrinkType } from './DrinkType';
import { Location } from './Location';
import { ActivityType } from './ActivityType';
import { ReactionDTO } from './ReactionDTO';
import { CommentReactionDTO } from './CommentReactionDTO';
import { UserInfo } from './UserInfo';
export class ActivityDTO {
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
    likes: ReactionDTO[];
    cheers: ReactionDTO[];
    comments: CommentReactionDTO[];
    registrationUser?: UserInfo;
    originalUserName?: string;

    getUserInfo(): UserInfo {
        const userInfo: UserInfo = {
            userId: this.userId,
            userName: this.userName
        };

        return userInfo;
    }
}
