import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { NoFriendsComponent } from './no-friends.component';

describe('NoFriendsComponent', () => {
  let component: NoFriendsComponent;
  let fixture: ComponentFixture<NoFriendsComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ NoFriendsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(NoFriendsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
