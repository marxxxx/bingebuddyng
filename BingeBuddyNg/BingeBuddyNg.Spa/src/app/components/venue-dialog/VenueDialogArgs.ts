import { LocationDTO } from 'src/models/LocationDTO';
import { VenueDialogMode } from './VenueDialogMode';
export class VenueDialogArgs {
    location: LocationDTO;
    mode:  VenueDialogMode = VenueDialogMode.Default;
}
