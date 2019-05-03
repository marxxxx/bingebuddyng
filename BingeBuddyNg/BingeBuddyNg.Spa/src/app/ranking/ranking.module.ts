import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { RankingComponent } from './ranking/ranking.component';
import { AuthGuard } from '../core/auth.guard';
import { SharedModule } from '../shared/shared.module';
import { RankingService } from './ranking.service';

const routes: Routes = [ {
  path: '',
  component: RankingComponent,
  canActivate: [AuthGuard]
}];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(routes)],
  exports: [],
  declarations: [RankingComponent],
  providers: [RankingService]
})
export class RankingModule { }
