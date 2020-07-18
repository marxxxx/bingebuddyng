import { VenueDTO } from 'src/models/VenueDTO';
import { Location } from './Location';
export class AddActivityBaseDTO {
    location?: Location;
    venue?: VenueDTO;
}
