import { VenueDTO } from 'src/models/VenueDTO';
export class VenueDialogResult {
    venue?: VenueDTO;
    action: 'leave' | 'change' | 'cancel';
}
