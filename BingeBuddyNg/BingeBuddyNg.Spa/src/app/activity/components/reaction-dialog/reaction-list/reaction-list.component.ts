import { Component, OnInit, Input } from '@angular/core';
import { ReactionDTO } from 'src/models/ReactionDTO';

@Component({
  selector: 'app-reaction-list',
  templateUrl: './reaction-list.component.html',
  styleUrls: ['./reaction-list.component.css']
})
export class ReactionListComponent implements OnInit {

  @Input()
  reactions: ReactionDTO[];

  constructor() { }

  ngOnInit() {
  }

}
