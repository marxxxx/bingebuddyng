import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { BingemapComponent } from './components/bingemap/bingemap.component';
import { AgmCoreModule } from '@agm/core';
import { SharedModule } from '../@shared/shared.module';
import { credentials } from 'src/environments/credentials';

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
      apiKey: credentials.googleMapsApiKey
    })
  ],
  exports: [],
  declarations: [BingemapComponent]
})
export class DrinkMapModule {}
