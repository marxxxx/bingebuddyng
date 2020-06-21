import { Component, OnInit } from '@angular/core';
import { DrinkService } from 'src/app/drinks/services/drink.service';
import { DrinkDTO } from 'src/models/DrinkDTO';
import { ShellInteractionService } from 'src/app/@core/services/shell-interaction.service';
import { ConfirmationDialogArgs } from 'src/app/@shared/components/confirmation-dialog/ConfirmationDialogArgs';
import { filter } from 'rxjs/operators';
import { DrinkActivityService } from 'src/app/activity/services/drink-activity.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-drinks',
  templateUrl: './drinks.component.html',
  styleUrls: ['./drinks.component.css']
})
export class DrinksComponent implements OnInit {

  drinks: DrinkDTO[] = [];
  isBusy = false;

  constructor(private drinkService: DrinkService,
    private drinkActivityService: DrinkActivityService,
    private router: Router,
    private shellInteraction: ShellInteractionService) { }

  ngOnInit() {
    this.load();
  }

  private load() {
    this.isBusy = true;
    this.drinkService.getDrinks().subscribe(d => {
      this.drinks = d;
      this.isBusy = false;
    }, e => {
      console.error(e);
      this.shellInteraction.showErrorMessage();
    });
  }

  onDelete(d: DrinkDTO) {
    const args: ConfirmationDialogArgs = {
      title: 'DeleteDrink',
      icon: 'delete',
      message: 'ReallyDeleteDrink',
      confirmButtonCaption: 'Delete',
      cancelButtonCaption: 'Cancel'
    };

    this.shellInteraction.showConfirmationDialog(args)
      .pipe(filter(isConfirmed => isConfirmed)).subscribe(() => {
        this.drinkService.deleteDrink(d.id).subscribe(() => {
          this.load();
        }, e => {
          console.error(e);
        });
      });
  }

  onDrink(d: DrinkDTO) {
    this.drinkActivityService.drink(d).subscribe(_ => this.router.navigateByUrl('/activity-feed'),
      e => console.error('Error drinking drink', d, e));
  }

}
