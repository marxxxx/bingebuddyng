import { LocationDTO } from './LocationDTO';
export class ActivityDTO {
    id: string;
    timestamp: Date;
    userProfileImageUrl: string;
    userName: string;
    message: string;
    locationAddress: string;
    location: LocationDTO;
}
