import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-drink-stats',
  templateUrl: './drink-stats.component.html',
  styleUrls: ['./drink-stats.component.css']
})
export class DrinkStatsComponent implements OnInit {

  @Input()
  drinkCount: number;

  @Input()
  alcLevel: number;

  constructor() { }

  ngOnInit(): void {
  }

}
