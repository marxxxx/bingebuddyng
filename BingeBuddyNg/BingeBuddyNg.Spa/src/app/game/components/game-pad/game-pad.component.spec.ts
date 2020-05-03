import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GamePadComponent } from './game-pad.component';

describe('GamePadComponent', () => {
  let component: GamePadComponent;
  let fixture: ComponentFixture<GamePadComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GamePadComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GamePadComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
