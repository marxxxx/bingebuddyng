import { VenueModel } from 'src/models/VenueModel';
import { UserInfoDTO } from './UserInfoDTO';
import { PushInfo } from './PushInfo';

export class UserDTO {
    id: string;
    name: string;
    profileImageUrl: string;
    weight?: number;
    pushInfo?: PushInfo;
    friends?: UserInfoDTO[];
    mutedFriendUserIds?: string[];
    currentVenue?: VenueModel;
    language?: string;
}
