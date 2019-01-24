import { VenueModel } from 'src/models/VenueModel';
export class VenueDialogResult {
    venue?: VenueModel;
    action: 'leave' | 'change' | 'cancel';
}
