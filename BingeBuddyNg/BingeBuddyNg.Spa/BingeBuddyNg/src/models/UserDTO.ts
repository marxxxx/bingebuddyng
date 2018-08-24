import { PushInfo } from "./PushInfo";

export class UserDTO {
    id: string;
    name: string;
    profileImageUrl: string;
    weight?: number;
    pushInfo?: PushInfo;
}
