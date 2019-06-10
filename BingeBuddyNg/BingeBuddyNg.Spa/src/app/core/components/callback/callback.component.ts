import { AuthService } from '../../services/auth.service';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-callback',
  templateUrl: './callback.component.html',
  styleUrls: ['./callback.component.css']
})
export class CallbackComponent implements OnInit {


  constructor(private auth: AuthService, private activatedRoute: ActivatedRoute) { }

  ngOnInit() {

    this.activatedRoute.paramMap.subscribe(p => {


      // get return url from route parameters or default to '/'
      const snapshot = this.activatedRoute.snapshot;
      const returnUrl = snapshot.queryParams['returnUrl'];

      this.auth.handleAuthentication(returnUrl);

    });

  }
}
