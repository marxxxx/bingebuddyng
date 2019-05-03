import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DrinkIconComponent } from './drink-icon.component';

describe('DrinkIconComponent', () => {
  let component: DrinkIconComponent;
  let fixture: ComponentFixture<DrinkIconComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DrinkIconComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DrinkIconComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
