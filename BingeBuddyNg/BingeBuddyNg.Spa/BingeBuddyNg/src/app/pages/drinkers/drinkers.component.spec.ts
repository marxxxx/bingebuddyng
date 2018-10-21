import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DrinkersComponent } from './drinkers.component';

describe('DrinkersComponent', () => {
  let component: DrinkersComponent;
  let fixture: ComponentFixture<DrinkersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DrinkersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DrinkersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
