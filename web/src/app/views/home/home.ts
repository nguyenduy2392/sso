import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

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
  private router = inject(Router);
  user = this.authService.getStoredUser();

  logout() {
    this.authService.logout();
    this.router.navigate(['/auth/login']);
  }
}
