import { Component, OnInit } from '@angular/core';
import { OpenIdConnectService } from '../open-id-connect.service';

@Component({
  selector: 'ac-redirect-silent-renew',
  templateUrl: './redirect-silent-renew.component.html',
  styleUrls: ['./redirect-silent-renew.component.scss']
})
export class RedirectSilentRenewComponent implements OnInit {
  constructor(private oidc: OpenIdConnectService) {}

  ngOnInit() {
    this.oidc.handleSilentCallback();
  }
}
