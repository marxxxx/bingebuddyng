import { VenueDTO } from 'src/models/VenueDTO';
import { UserInfoDTO } from './UserInfoDTO';
import { PushInfo } from './PushInfo';
import { FriendRequestDTO } from './FriendRequestDTO';

export class UserDTO {
    id: string;
    name: string;
    profileImageUrl: string;
    weight?: number;
    pushInfo?: PushInfo;
    friends?: UserInfoDTO[];
    currentVenue?: VenueDTO;
    language?: string;
    incomingFriendRequests?: FriendRequestDTO[];
    outgoingFriendRequests?: FriendRequestDTO[];
}
