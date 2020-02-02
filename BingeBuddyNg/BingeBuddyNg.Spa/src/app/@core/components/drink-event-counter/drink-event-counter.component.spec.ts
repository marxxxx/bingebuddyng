import { DrinkEvent } from './../../../../models/DrinkEvent';
import { DrinkEventCounterComponent } from './drink-event-counter.component';
import { of, BehaviorSubject } from 'rxjs';
import { tick, fakeAsync } from '@angular/core/testing';


describe('DrinkEventCounter', () => {

  let component: DrinkEventCounterComponent;

  beforeEach(() => {
    const authService: any = {
      isLoggedIn$: of(true),
      currentUserProfile$: new BehaviorSubject({ sub: '123' })
    };
    const notificationService: any = {
      activityReceived$: of()
    };

    const drinkEventService = jasmine.createSpyObj(['getCurrentDrinkEvent']);
    drinkEventService.currentUserScored$ = of();

    const now = new Date();

    const drinkEvent: DrinkEvent = {
      startUtc: now.toISOString(),
      endUtc: new Date(now.getTime() + (1 * 60000)).toISOString(),
      scoringUserIds: []
    };
    drinkEventService.getCurrentDrinkEvent.and.returnValue(of(drinkEvent));

    component = new DrinkEventCounterComponent(authService, drinkEventService, notificationService);
    expect(component).toBeTruthy();
    component.ngOnInit();
  });

  it('should be visible if drink event is active', () => {
    expect(component.isVisible()).toBeTruthy();
  });

  it('should display correct time initially', () => {
    component.updateRemainingTime();

    expect(component.remainingTime).toBe('01:00');
  });

  it('should reduce time after a second', fakeAsync(() => {
    tick(1000);
    component.updateRemainingTime();
    expect(component.remainingTime).toBe('00:59');
  }));
});
