import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-no-friends',
  templateUrl: './no-friends.component.html',
  styleUrls: ['./no-friends.component.css']
})
export class NoFriendsComponent implements OnInit {

  @Input()
  showEnterDrinkHint = true;

  constructor() { }

  ngOnInit() {
  }

}
