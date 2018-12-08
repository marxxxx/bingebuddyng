import { UserInfo } from './UserInfo';

export class UserProfile {
    family_name: string;
    given_name: string;
    name: string;
    nickname: string;
    picture: string;
    sub: string;
    updated_at: any;

    toUserInfo(): UserInfo {
        return { userId: this.sub, userName: this.nickname };
    }
}
