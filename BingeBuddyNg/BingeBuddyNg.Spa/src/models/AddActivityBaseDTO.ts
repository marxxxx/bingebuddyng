import { VenueModel } from 'src/models/VenueModel';
import { Location } from './Location';
export class AddActivityBaseDTO {
    location?: Location;
    venue?: VenueModel;
}
