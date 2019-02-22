import { TestBed } from '@angular/core/testing';

import { OpenIdConnectService } from './open-id-connect.service';

describe('OpenIdConnectService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: OpenIdConnectService = TestBed.get(OpenIdConnectService);
    expect(service).toBeTruthy();
  });
});
