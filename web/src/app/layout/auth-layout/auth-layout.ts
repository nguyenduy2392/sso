import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';

@Component({
  selector: 'app-auth-layout',
  standalone: true,
  imports: [RouterModule],
  template: `
    <div class="sso-page">
      <!-- Left: Product showcase -->
      <div class="sso-showcase">
        <div class="sso-showcase-inner">
          <!-- Logo -->

          <!-- Headline -->
          <div class="sso-showcase-headline">
            <h1>Nền tảng quản lý doanh nghiệp hiệu quả</h1>
            <p>Đăng nhập một lần, truy cập mọi ứng dụng trong hệ sinh thái</p>
          </div>

          <!-- Product cards -->
          <div class="sso-products">
            <!-- TaskFlow -->
            <div class="sso-product-card">
              <div class="sso-product-icon sso-product-icon--blue">
                <i class="bi bi-kanban"></i>
              </div>
              <div class="sso-product-body">
                <h3>TaskFlow</h3>
                <p>Quản lý công việc & tăng hiệu suất làm việc cho đội nhóm</p>
                <div class="sso-product-features">
                  <span><i class="bi bi-check-circle-fill"></i> Dashboard tổng quan</span>
                  <span><i class="bi bi-check-circle-fill"></i> Theo dõi tiến độ</span>
                  <span><i class="bi bi-check-circle-fill"></i> Báo cáo tự động</span>
                </div>
              </div>
              <div class="sso-product-qr">
                <img src="assets/images/qr-taskflow.png" alt="QR TaskFlow" />
                <span>Tải App</span>
              </div>
            </div>

            <!-- Mini CRM -->
            <div class="sso-product-card">
              <div class="sso-product-icon sso-product-icon--green">
                <i class="bi bi-graph-up-arrow"></i>
              </div>
              <div class="sso-product-body">
                <h3>Mini CRM</h3>
                <p>Quản lý quan hệ khách hàng & tăng doanh thu bán hàng</p>
                <div class="sso-product-features">
                  <span><i class="bi bi-check-circle-fill"></i> Pipeline bán hàng</span>
                  <span><i class="bi bi-check-circle-fill"></i> Quản lý hợp đồng</span>
                  <span><i class="bi bi-check-circle-fill"></i> Phân tích doanh thu</span>
                </div>
              </div>
            </div>
          </div>

          <!-- Background decoration -->
          <div class="sso-showcase-decor">
            <div class="sso-decor-circle sso-decor-circle--1"></div>
            <div class="sso-decor-circle sso-decor-circle--2"></div>
            <div class="sso-decor-circle sso-decor-circle--3"></div>
          </div>
        </div>
      </div>

      <!-- Right: Login form -->
      <div class="sso-form-side">
        <div class="sso-form-container">
          <div class="sso-form-logo d-lg-none text-center mb-4">
            <img src="assets/images/logo-dark.png" alt="logo" height="40" />
          </div>
          <router-outlet />
        </div>
      </div>
    </div>
  `,
})
export class AuthLayout {}
