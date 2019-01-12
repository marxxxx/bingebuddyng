import { VenueModel } from 'src/models/VenueModel';
import { UserInfo } from './UserInfo';
import { PushInfo } from './PushInfo';

export class User {
    id: string;
    name: string;
    profileImageUrl: string;
    weight?: number;
    pushInfo?: PushInfo;
    friends?: UserInfo[];
    mutedFriendUserIds?: string[];
    currentVenue?: VenueModel;
}
