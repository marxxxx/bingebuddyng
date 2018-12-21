import { UserRanking } from './../../../models/UserRanking';
import { RankingService } from './../../services/ranking.service';
import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-ranking',
  templateUrl: './ranking.component.html',
  styleUrls: ['./ranking.component.css']
})
export class RankingComponent implements OnInit {

  ranking$: Observable<UserRanking[]>;
  score$: Observable<UserRanking[]>;

  constructor(private rankingService: RankingService) { }

  ngOnInit() {
    this.ranking$ = this.rankingService.getRanking();
    this.score$ = this.rankingService.getRanking();
  }

}
