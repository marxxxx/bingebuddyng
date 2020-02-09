import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DrinkAnimationComponent } from './drink-animation.component';

describe('DrinkAnimationComponent', () => {
  let component: DrinkAnimationComponent;
  let fixture: ComponentFixture<DrinkAnimationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DrinkAnimationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DrinkAnimationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
