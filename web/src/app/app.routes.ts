import { inject } from '@angular/core';
import { Router, Routes } from '@angular/router';
import { AuthLayout } from './layout/auth-layout/auth-layout';
import { AuthService } from './services/auth.service';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthLayout,
    loadChildren: () => import('./views/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },
  {
    path: '',
    canActivate: [
      (_route: any, state: any) => {
        const router = inject(Router);
        const auth = inject(AuthService);
        if (!auth.isLoggedIn()) {
          return router.createUrlTree(['/auth/login'], { queryParams: { returnUrl: state.url } });
        }
        return true;
      },
    ],
    loadChildren: () => import('./views/home/home.routes').then((m) => m.HOME_ROUTES),
  },
  { path: '**', redirectTo: '' },
];
