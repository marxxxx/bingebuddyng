import { UserRankingDTO } from '../../../../models/UserRankingDTO';
import { RankingService } from '../../services/ranking.service';
import { Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
import { VenueRankingDTO } from 'src/models/VenueRankingDTO';
import { ShellInteractionService } from 'src/app/core/services/shell-interaction.service';

@Component({
  selector: 'app-ranking',
  templateUrl: './ranking.component.html',
  styleUrls: ['./ranking.component.css']
})
export class RankingComponent implements OnInit {

  isBusy = false;
  ranking: UserRankingDTO[];
  score: UserRankingDTO[];
  venueRanking: VenueRankingDTO[];

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
