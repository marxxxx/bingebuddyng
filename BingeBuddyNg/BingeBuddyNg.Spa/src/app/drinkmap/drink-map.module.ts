import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BingemapComponent } from './components/bingemap/bingemap.component';
import { AgmCoreModule } from '@agm/core';
import { SharedModule } from '../@shared/shared.module';

const routes: Routes = [
  {
    path: '',
    component: BingemapComponent
  }
];

@NgModule({
  imports: [
    SharedModule,
    RouterModule.forChild(routes),
    AgmCoreModule.forRoot({
      apiKey: 'AIzaSyBlGceFBW7ykKMzNH4o0DwMBlxwt8NgWc8'
    })
  ],
  exports: [],
  declarations: [BingemapComponent]
})
export class DrinkMapModule {}
