import { AgmCoreModule } from '@agm/core';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { AppMaterialModule } from '../../modules/app-material.module';
import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BingemapComponent } from './bingemap.component';
import { ProgressSpinnerComponent } from 'src/app/components/progress-spinner/progress-spinner.component';
import { UserInfoComponent } from 'src/app/components/user-info/user-info.component';
import { DrinkIconComponent } from 'src/app/components/drink-icon/drink-icon.component';
import { RouterTestingModule } from '@angular/router/testing';

describe('BingemapComponent', () => {
  let component: BingemapComponent;
  let fixture: ComponentFixture<BingemapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      imports: [AppMaterialModule, FormsModule, HttpClientModule, AgmCoreModule, RouterTestingModule],
      declarations: [ BingemapComponent, ProgressSpinnerComponent, UserInfoComponent, DrinkIconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BingemapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
