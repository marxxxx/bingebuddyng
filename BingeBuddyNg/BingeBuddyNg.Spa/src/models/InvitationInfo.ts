import { UserInfoDTO } from './UserInfoDTO';

export class InvitationDTO {
    invitationToken: string;
    invitingUserId: string;
    invitingUser: UserInfoDTO;
}
