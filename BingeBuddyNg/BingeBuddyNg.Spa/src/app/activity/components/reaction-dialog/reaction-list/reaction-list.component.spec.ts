import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReactionListComponent } from './reaction-list.component';

describe('ReactionListComponent', () => {
  let component: ReactionListComponent;
  let fixture: ComponentFixture<ReactionListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReactionListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReactionListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
