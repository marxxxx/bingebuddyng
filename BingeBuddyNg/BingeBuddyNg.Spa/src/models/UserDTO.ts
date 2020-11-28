import { VenueDTO } from 'src/models/VenueDTO';
import { UserInfoDTO } from './UserInfoDTO';
import { PushInfo } from './PushInfo';
import { FriendRequestDTO } from './FriendRequestDTO';
import { UserStatisticsDTO } from './UserStatisticsDTO';

export class UserDTO {
    id: string;
    name: string;
    profileImageUrl: string;
    weight?: number;
    pushInfo?: PushInfo;
    friends?: UserInfoDTO[];
    currentVenue?: VenueDTO;
    language?: string;
    currentStats: UserStatisticsDTO;
    incomingFriendRequests?: FriendRequestDTO[];
    outgoingFriendRequests?: FriendRequestDTO[];
}
