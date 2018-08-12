import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { BingemapComponent } from './bingemap.component';

describe('BingemapComponent', () => {
  let component: BingemapComponent;
  let fixture: ComponentFixture<BingemapComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ BingemapComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(BingemapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
