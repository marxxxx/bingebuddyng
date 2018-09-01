import { Component, OnInit, Input } from '@angular/core';

@Component({
  selector: 'app-progress-spinner',
  templateUrl: './progress-spinner.component.html',
  styleUrls: ['./progress-spinner.component.scss']
})
export class ProgressSpinnerComponent implements OnInit {

  //////////////////////////////////////////////////////////////////////////////
  // interface
  //////////////////////////////////////////////////////////////////////////////
  @Input() labelText = '';
  @Input() isBackgroundTransparent: boolean;

  //////////////////////////////////////////////////////////////////////////////
  // construction
  //////////////////////////////////////////////////////////////////////////////
  constructor() { }

  //////////////////////////////////////////////////////////////////////////////
  // functions
  //////////////////////////////////////////////////////////////////////////////
  ngOnInit() {
  }

  getBackground(): string {
    return this.isBackgroundTransparent ? null : 'rgba(0,0,0,0.288)';
  }
}
