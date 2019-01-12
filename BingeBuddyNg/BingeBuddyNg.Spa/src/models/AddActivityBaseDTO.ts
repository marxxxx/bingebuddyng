import { VenueModel } from 'src/models/VenueModel';
import { LocationDTO } from './LocationDTO';
export class AddActivityBaseDTO {
    location?: LocationDTO;
    venue?: VenueModel;
}
