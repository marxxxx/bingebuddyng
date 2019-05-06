import { NgModule } from '@angular/core';

import { StatsComponent } from './components/stats/stats.component';
import { DrinkChartComponent } from './components/drink-chart/drink-chart.component';
import { DrinkRatioChartComponent } from './components/drink-ratio-chart/drink-ratio-chart.component';
import { Routes, RouterModule } from '@angular/router';
import { AuthGuard } from '../core/services/auth.guard';
import { ChartsModule } from 'ng2-charts';
import { SharedModule } from '../shared/shared.module';

const routes: Routes = [{
  path: '',
  component: StatsComponent,
  canActivate: [AuthGuard]
}];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(routes), ChartsModule],
  exports: [],
  declarations: [StatsComponent, DrinkChartComponent, DrinkRatioChartComponent]
})
export class StatisticsModule {}
