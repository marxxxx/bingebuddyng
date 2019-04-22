import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { Drink } from 'src/models/Drink';
import { DrinkService } from 'src/app/services/drink.service';
import { ShellInteractionService } from 'src/app/services/shell-interaction.service';

@Component({
  selector: 'app-add-or-edit-drink',
  templateUrl: './add-or-edit-drink.component.html',
  styleUrls: ['./add-or-edit-drink.component.css']
})
export class AddOrEditDrinkComponent implements OnInit {
  form: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required]),
    alcPrc: new FormControl(0, [Validators.required]),
    volume: new FormControl(0, [Validators.required]),
    drinkType: new FormControl('', [Validators.required])
  });

  drink: Drink;
  isBusy = false;

  constructor(route: ActivatedRoute,
    private drinkService: DrinkService,
    private shellInteraction: ShellInteractionService) {
    route.paramMap.subscribe(p => {
      if (p.has('drinkId')) {
        this.load(p.get('drinkId'));
      }
    });
  }

  ngOnInit() {}

  load(drinkId: string) {
    console.log('loading drink id ', drinkId);

    this.drinkService.getDrink(drinkId).subscribe( d=> {
      this.drink = d;
      this.form.controls.name.setValue(d.name);
      this.form.controls.drinkType.setValue(d.drinkType);
      this.form.controls.alcPrc.setValue(d.alcPrc);
      this.form.controls.volume.setValue(d.volume);
      this.isBusy = false;
    }, e=> {
      this.isBusy = false;
      console.error(e);
      this.shellInteraction.showErrorMessage();
    })

  }
}
