import { Routes } from '@angular/router';
import { Login } from './login/login';
import { Authorize } from './authorize/authorize';

export const AUTH_ROUTES: Routes = [
  { path: 'login', component: Login, title: 'Đăng nhập' },
  { path: 'authorize', component: Authorize, title: 'Xác thực' },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
];
