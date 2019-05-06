import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Drink } from 'src/models/Drink';
import { DrinkService } from 'src/app/drinks/services/drink.service';
import { ShellInteractionService } from 'src/app/core/services/shell-interaction.service';
import { DrinkType } from 'src/models/DrinkType';

@Component({
  selector: 'app-add-or-edit-drink',
  templateUrl: './add-or-edit-drink.component.html',
  styleUrls: ['./add-or-edit-drink.component.css']
})
export class AddOrEditDrinkComponent implements OnInit {
  form: FormGroup = new FormGroup({
    name: new FormControl('', [Validators.required, Validators.maxLength(50)]),
    alcPrc: new FormControl(5, [Validators.required, Validators.min(0), Validators.max(90)]),
    volume: new FormControl(500, [Validators.required, Validators.min(20), Validators.max(1000)]),
    drinkType: new FormControl(DrinkType.Beer, [Validators.required])
  });

  drink: Drink;
  isBusy = false;
  isAdd = false;

  drinkTypes = [{
    type: DrinkType.Beer,
    name: 'Beer',
    defaultAlc: 5,
    defaultVol: 500
  }, {
    type: DrinkType.Wine,
    name: 'Wine',
    defaultAlc: 9,
    defaultVol: 125
  }, {
    type: DrinkType.Shot,
    name: 'Shot',
    defaultAlc: 40,
    defaultVol: 20
  }, {
    type: DrinkType.Anti,
    name: 'Anti',
    defaultAlc: 0,
    defaultVol: 250
  }];

  isSaving: boolean;

  constructor(route: ActivatedRoute,
    private router: Router,
    private drinkService: DrinkService,
    private shellInteraction: ShellInteractionService) {
    route.paramMap.subscribe(p => {
      if (p.has('drinkId')) {
        this.load(p.get('drinkId'));
      }

      route.url.subscribe(u => this.isAdd = u.toString().indexOf('add') >= 0);
    });


  }

  ngOnInit() {
    this.form.controls.drinkType.valueChanges.subscribe(v => {
      console.log('value changed', v);
      const drinkType = this.drinkTypes.filter(d => d.type === <DrinkType>v)[0];
      console.log('DrinkType', drinkType);
      this.form.controls.alcPrc.setValue(drinkType.defaultAlc);
      this.form.controls.volume.setValue(drinkType.defaultVol);

    });
  }

  load(drinkId: string) {
    console.log('loading drink id ', drinkId);

    this.drinkService.getDrink(drinkId).subscribe(d => {
      this.drink = d;
      this.form.controls.name.setValue(d.name);
      this.form.controls.drinkType.setValue(d.drinkType);
      this.form.controls.alcPrc.setValue(d.alcPrc);
      this.form.controls.volume.setValue(d.volume);
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      console.error(e);
      this.shellInteraction.showErrorMessage();
    });

  }

  onSave() {
    this.isSaving = true;
    this.drinkService.saveDrinks([{
      name: this.form.controls.name.value,
      drinkType: this.form.controls.drinkType.value,
      alcPrc: this.form.controls.alcPrc.value,
      volume: this.form.controls.volume.value
    }]).subscribe(s => {
      this.isSaving = false;
      this.router.navigateByUrl('/drinks');
    }, e => {
      this.isSaving = false;
      this.shellInteraction.showErrorMessage();
    });

  }
}
