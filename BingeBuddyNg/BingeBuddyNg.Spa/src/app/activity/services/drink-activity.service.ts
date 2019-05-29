import { MatDialog } from '@angular/material/dialog';
import { ActivityService } from './activity.service';
import { LocationService } from './location.service';
import { Injectable } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { AddDrinkActivityDTO } from 'src/models/AddDrinkActivityDTO';
import { Drink } from 'src/models/Drink';
import { DrinkDialogComponent } from '../components/drink-dialog/drink-dialog.component';
import { DrinkType } from 'src/models/DrinkType';
import { DrinkDialogArgs } from '../components/drink-dialog/DrinkDialogArgs';

@Injectable({providedIn: 'root'})
export class DrinkActivityService {


  constructor(private locationService: LocationService,
    private activityService: ActivityService,
    private dialog: MatDialog) { }

  drink(drink: Drink): Observable<any> {
    const activity: AddDrinkActivityDTO = {
      drinkId: drink.id,
      drinkType: drink.drinkType,
      drinkName: drink.name,
      alcPrc: drink.alcPrc,
      volume: drink.volume,
      location: this.locationService.getCurrentLocation(),
      venue: this.locationService.getCurrentVenue()
    };

    return forkJoin([this.displayDrinkDialog(drink.drinkType),
      this.activityService.addDrinkActivity(activity)]);
  }


  displayDrinkDialog(type: DrinkType): Observable<any> {
    const args: DrinkDialogArgs = { drinkType: type };
    return this.dialog.open(DrinkDialogComponent, { data: args, width: '90%' }).afterClosed();
  }
}
