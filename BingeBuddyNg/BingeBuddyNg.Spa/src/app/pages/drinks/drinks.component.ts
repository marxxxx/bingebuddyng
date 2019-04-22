import { Component, OnInit } from '@angular/core';
import { DrinkService } from 'src/app/services/drink.service';
import { Drink } from 'src/models/Drink';
import { ShellInteractionService } from 'src/app/services/shell-interaction.service';

@Component({
  selector: 'app-drinks',
  templateUrl: './drinks.component.html',
  styleUrls: ['./drinks.component.css']
})
export class DrinksComponent implements OnInit {

  drinks: Drink[] = [];
  isBusy = false;

  constructor(private drinkService: DrinkService,
    private shellInteraction: ShellInteractionService) { }

  ngOnInit() {
    this.isBusy = true;
    this.drinkService.getDrinks().subscribe( d => {
      this.drinks = d;
      this.isBusy = false;
    }, e => {
      console.error(e);
      this.shellInteraction.showErrorMessage();
    });
  }

}
