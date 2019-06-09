import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ReactionDialogComponent } from './reaction-dialog.component';

describe('ReactionDialogComponent', () => {
  let component: ReactionDialogComponent;
  let fixture: ComponentFixture<ReactionDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ReactionDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ReactionDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
