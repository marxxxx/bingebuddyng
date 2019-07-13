import { PushInfo } from './PushInfo';

export class CreateOrUpdateUserDTO {

    constructor(public userId: string,
        public name: string,
        public profileImageUrl: string,
        public pushInfo?: PushInfo,
        public language?: string) {

    }
}
