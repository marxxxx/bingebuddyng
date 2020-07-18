import { UserInfoDTO } from './UserInfoDTO';
import { FriendRequestDirection } from './FriendRequestDirection';
export class FriendRequestDTO {
  requestTimestamp: any;

  user: UserInfoDTO;

  direction: FriendRequestDirection;
}
