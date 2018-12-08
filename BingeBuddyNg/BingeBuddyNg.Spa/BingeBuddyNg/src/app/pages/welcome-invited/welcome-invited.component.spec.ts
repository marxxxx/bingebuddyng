import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WelcomeInvitedComponent } from './welcome-invited.component';

describe('WelcomeInvitedComponent', () => {
  let component: WelcomeInvitedComponent;
  let fixture: ComponentFixture<WelcomeInvitedComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WelcomeInvitedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WelcomeInvitedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
