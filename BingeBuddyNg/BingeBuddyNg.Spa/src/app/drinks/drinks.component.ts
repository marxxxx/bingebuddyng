import { Component, OnInit } from '@angular/core';
import { DrinkService } from 'src/app/drinks/drink.service';
import { Drink } from 'src/models/Drink';
import { ShellInteractionService } from 'src/app/services/shell-interaction.service';
import { ConfirmationDialogArgs } from 'src/app/components/confirmation-dialog/ConfirmationDialogArgs';
import { filter } from 'rxjs/operators';
import { DrinkActivityService } from 'src/app/services/drink-activity.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-drinks',
  templateUrl: './drinks.component.html',
  styleUrls: ['./drinks.component.css']
})
export class DrinksComponent implements OnInit {

  drinks: Drink[] = [];
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

  onDelete(d: Drink) {
    const args: ConfirmationDialogArgs = {
      title: 'DeleteDrink',
      icon: 'delete',
      message: 'ReallyDeleteDrink',
      confirmButtonCaption: 'Delete',
      cancelButtonCaption: 'Cancel'
    };

    this.shellInteraction.showConfirmationDialog(args)
      .pipe(filter(isConfirmed => isConfirmed)).subscribe(_ => {
        this.drinkService.deleteDrink(d.id).subscribe(_ => {
          this.load();
        }, e => {
          console.error(e);
        });
      });
  }

  onDrink(d: Drink) {
    this.drinkActivityService.drink(d).subscribe(_ => this.router.navigateByUrl('/activity'),
      e => console.error('Error drinking drink', d, e));
  }

}
