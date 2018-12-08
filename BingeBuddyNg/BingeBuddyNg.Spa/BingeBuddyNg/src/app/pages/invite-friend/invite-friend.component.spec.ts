import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { InviteFriendComponent } from './invite-friend.component';

describe('InviteFriendComponent', () => {
  let component: InviteFriendComponent;
  let fixture: ComponentFixture<InviteFriendComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ InviteFriendComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(InviteFriendComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
