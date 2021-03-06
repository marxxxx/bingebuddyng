import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { AgmCoreModule } from '@agm/core';

import { SharedModule } from '../@shared/shared.module';
import { environment } from 'src/environments/environment';
import { BingemapComponent } from './components/bingemap/bingemap.component';

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
      apiKey: environment.credentials.googleMapsApiKey
    })
  ],
  exports: [],
  declarations: [BingemapComponent]
})
export class DrinkMapModule {}
