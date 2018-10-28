import { DrinkType } from 'src/models/DrinkType';
import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-drink-icon',
  templateUrl: './drink-icon.component.html',
  styleUrls: ['./drink-icon.component.css']
})
export class DrinkIconComponent implements OnInit {

  @Input()
  drinkType: DrinkType;

  constructor() { }

  ngOnInit() {
  }

}
