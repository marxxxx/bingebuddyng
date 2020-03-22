import { MatDialog } from '@angular/material/dialog';
import { ActivityService } from './activity.service';
import { LocationService } from './location.service';
import { Injectable } from '@angular/core';
import { Observable, forkJoin, timer } from 'rxjs';
import { AddDrinkActivityDTO } from 'src/models/AddDrinkActivityDTO';
import { Drink } from 'src/models/Drink';
import { DrinkType } from 'src/models/DrinkType';
import { DrinkDialogArgs } from '../components/drink-dialog/DrinkDialogArgs';
import { DrinkDialogComponent } from '../components/drink-dialog/drink-dialog.component';

@Injectable({ providedIn: 'root' })
export class DrinkActivityService {

  constructor(private locationService: LocationService,
    private activityService: ActivityService,
    private dialog: MatDialog) { }

  drink(drink: Drink, displayDialog: boolean = true): Observable<[any, string]> {
    const activity: AddDrinkActivityDTO = {
      drinkId: drink.id,
      drinkType: drink.drinkType,
      drinkName: drink.name,
      alcPrc: drink.alcPrc,
      volume: drink.volume,
      location: this.locationService.getCurrentLocation(),
      venue: this.locationService.getCurrentVenue()
    };

    const observable = displayDialog ?
      forkJoin([this.displayDrinkDialog(drink.drinkType), this.activityService.addDrinkActivity(activity)]) :
      forkJoin([timer(5000), this.activityService.addDrinkActivity(activity)]);

    return observable;
  }


  displayDrinkDialog(type: DrinkType): Observable<any> {
    const args: DrinkDialogArgs = { drinkType: type };
    return this.dialog.open(DrinkDialogComponent, { data: args, width: '90%' }).afterClosed();
  }
}
