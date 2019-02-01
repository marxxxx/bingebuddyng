import { Location } from 'src/models/Location';
import { VenueDialogMode } from './VenueDialogMode';
export class VenueDialogArgs {
    location: Location;
    mode:  VenueDialogMode = VenueDialogMode.Default;
}
