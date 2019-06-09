import { Component, OnInit, Input } from '@angular/core';
import { Reaction } from 'src/models/Reaction';

@Component({
  selector: 'app-reaction-list',
  templateUrl: './reaction-list.component.html',
  styleUrls: ['./reaction-list.component.css']
})
export class ReactionListComponent implements OnInit {

  @Input()
  reactions: Reaction[];

  constructor() { }

  ngOnInit() {
  }

}
