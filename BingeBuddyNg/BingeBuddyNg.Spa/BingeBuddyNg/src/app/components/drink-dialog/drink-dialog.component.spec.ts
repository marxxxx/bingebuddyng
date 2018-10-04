import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DrinkDialogComponent } from './drink-dialog.component';

describe('DrinkDialogComponent', () => {
  let component: DrinkDialogComponent;
  let fixture: ComponentFixture<DrinkDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DrinkDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DrinkDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
