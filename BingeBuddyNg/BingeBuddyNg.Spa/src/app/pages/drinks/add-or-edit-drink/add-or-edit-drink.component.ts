import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';

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

  constructor(route: ActivatedRoute) {
    route.paramMap.subscribe(p => {
      if (p.has('drinkId')) {
        this.load(p.get('drinkId'));
      }
    });
  }

  ngOnInit() {}

  load(drinkId: string) {
    console.log('loading drink id ', drinkId);

  }
}
