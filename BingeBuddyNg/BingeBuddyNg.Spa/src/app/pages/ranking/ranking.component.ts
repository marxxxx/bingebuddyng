import { UserRanking } from './../../../models/UserRanking';
import { RankingService } from './../../services/ranking.service';
import { Component, OnInit } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { VenueRanking } from 'src/models/VenueRanking';
import { ShellInteractionService } from 'src/app/services/shell-interaction.service';

@Component({
  selector: 'app-ranking',
  templateUrl: './ranking.component.html',
  styleUrls: ['./ranking.component.css']
})
export class RankingComponent implements OnInit {

  isBusy = false;
  ranking: UserRanking[];
  score: UserRanking[];
  venueRanking: VenueRanking[];

  constructor(private rankingService: RankingService,
    private shellInteractionService: ShellInteractionService) { }

  ngOnInit() {

    this.isBusy = true;

    forkJoin(
      this.rankingService.getRanking(),
      this.rankingService.getScores(),
      this.rankingService.getVenueRanking()
    ).subscribe(([ranking, score, venueRanking]) => {
      this.ranking = ranking;
      this.score = score;
      this.venueRanking = venueRanking;
      this.isBusy = false;
    }, e => {
      this.isBusy = false;
      this.shellInteractionService.showErrorMessage();
    });
  }

}
