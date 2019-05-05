import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { VenueDialogComponent } from './venue-dialog.component';

describe('VenueDialogComponent', () => {
  let component: VenueDialogComponent;
  let fixture: ComponentFixture<VenueDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ VenueDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(VenueDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
