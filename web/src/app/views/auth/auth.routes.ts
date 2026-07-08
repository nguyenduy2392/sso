import { inject } from '@angular/core';
import { CanActivateFn, Router, Routes } from '@angular/router';
import { Login } from './login/login';
import { Authorize } from './authorize/authorize';
import { AuthService } from '../../services/auth.service';

const redirectIfLoggedIn: CanActivateFn = (route) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isLoggedIn()) return true;

  const returnUrl = route.queryParams['returnUrl'];
  if (returnUrl) {
    window.location.href = decodeURIComponent(returnUrl);
    return false;
  }
  return router.createUrlTree(['/']);
};

export const AUTH_ROUTES: Routes = [
  { path: 'login', component: Login, title: 'Đăng nhập', canActivate: [redirectIfLoggedIn] },
  { path: 'authorize', component: Authorize, title: 'Xác thực' },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
