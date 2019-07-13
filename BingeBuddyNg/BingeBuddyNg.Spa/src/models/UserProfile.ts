import { UserInfoDTO } from './UserInfoDTO';

export class UserProfile {
    family_name: string;
    given_name: string;
    name: string;
    nickname: string;
    picture: string;
    sub: string;
    updated_at: any;

    toUserInfo(): UserInfoDTO {
        return { userId: this.sub, userName: this.nickname };
    }
}
