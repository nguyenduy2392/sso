import { Component, inject } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { EnvService } from '../../services/env.service';

@Component({
  standalone: true,
  template: `
    <div class="d-flex min-vh-100 justify-content-center align-items-center">
      <div class="text-center">
        <h4>Xin chào, {{ user?.userName }}</h4>
        <p class="text-muted">Bạn đã đăng nhập thành công.</p>
        <button class="btn btn-outline-danger btn-sm" (click)="logout()">Đăng xuất</button>
      </div>
    </div>
  `,
})
export class Home {
  private authService = inject(AuthService);
  private env = inject(EnvService);
  user = this.authService.getStoredUser();

  logout() {
    this.authService.logout();
    // Gọi BE để xoá sso_session cookie và fan-out /sso-logout tới các app khác
    const returnUrl = encodeURIComponent(window.location.origin + '/auth/login');
    window.location.href = `${this.env.apiUrl}/auth/logout?returnUrl=${returnUrl}`;
  }
}
