import { VenueModel } from 'src/models/VenueModel';
import { DrinkType } from './DrinkType';
import { Location } from './Location';
import { ActivityType } from './ActivityType';
import { ReactionDTO } from './ReactionDTO';
import { CommentReactionDTO } from './CommentReactionDTO';
import { UserInfoDTO } from './UserInfoDTO';
import { GameDTO } from 'src/app/game/models/GameDTO';

export class ActivityDTO {
    id: string;
    activityType: ActivityType;
    timestamp: Date;
    userId: string;
    userName: string;
    message?: string;
    locationAddress?: string;
    location?: Location;
    drinkType?: DrinkType;
    drinkName?: string;
    drinkCount?: number;
    alcLevel?: number;
    imageUrl?: string;
    venue?: VenueModel;
    likes: ReactionDTO[];
    cheers: ReactionDTO[];
    comments: CommentReactionDTO[];
    registrationUser?: UserInfoDTO;
    originalUserName?: string;
    gameInfo?: GameDTO;
}
