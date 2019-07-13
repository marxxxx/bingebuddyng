import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AddOrEditDrinkComponent } from './components/add-or-edit-drink/add-or-edit-drink.component';
import { DrinksComponent } from './components/drinks/drinks.component';
import { AuthGuard } from '../@core/services/auth.guard';
import { SharedModule } from '../@shared/shared.module';
import { DrinkService } from './services/drink.service';

const routes: Routes = [
  {
    path: '',
    component: DrinksComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'add',
    component: AddOrEditDrinkComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'edit/:drinkId',
    component: AddOrEditDrinkComponent,
    canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [SharedModule, RouterModule.forChild(routes)],
  exports: [],
  declarations: [DrinksComponent, AddOrEditDrinkComponent],
  providers: [DrinkService]
})
export class DrinksModule {}
