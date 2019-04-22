import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AddOrEditDrinkComponent } from './add-or-edit-drink.component';

describe('AddOrEditDrinkComponent', () => {
  let component: AddOrEditDrinkComponent;
  let fixture: ComponentFixture<AddOrEditDrinkComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AddOrEditDrinkComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AddOrEditDrinkComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
