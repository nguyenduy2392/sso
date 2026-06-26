import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { environment } from '../../../../environments/environment';

@Component({
  standalone: true,
  template: `<div class="d-flex min-vh-100 justify-content-center align-items-center">
    <div class="spinner-border text-primary"></div>
  </div>`,
})
export class Authorize implements OnInit {
  private route = inject(ActivatedRoute);

  ngOnInit(): void {
    const { client_id, redirect_uri, state } = this.route.snapshot.queryParams;
    const params = new URLSearchParams({ client_id, redirect_uri, state: state ?? '' });
    // Gọi BE authorize endpoint — BE sẽ redirect (302) sang redirect_uri hoặc trang login
    window.location.href = `${environment.apiUrl}/auth/authorize?${params}`;
  }
}
