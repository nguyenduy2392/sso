import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { EnvService } from '../../../services/env.service';

@Component({
  standalone: true,
  template: `<div class="d-flex min-vh-100 justify-content-center align-items-center">
    <div class="spinner-border text-primary"></div>
  </div>`,
})
export class Authorize implements OnInit {
  private route = inject(ActivatedRoute);
  private env = inject(EnvService);

  ngOnInit(): void {
    const { client_id, redirect_uri, state } = this.route.snapshot.queryParams;
    const params = new URLSearchParams({ client_id, redirect_uri, state: state ?? '' });
    window.location.href = `${this.env.apiUrl}/auth/authorize?${params}`;
  }
}
