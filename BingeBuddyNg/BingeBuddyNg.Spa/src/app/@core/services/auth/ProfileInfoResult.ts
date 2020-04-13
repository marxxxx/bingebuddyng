import { UserProfile } from 'src/models/UserProfile';

export class ProfileInfoResult {

  constructor(
    public profile: UserProfile,
    public isRegistered: boolean) {
  }
}
