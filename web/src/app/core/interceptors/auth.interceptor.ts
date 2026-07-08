import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const isAuthEndpoint = /\/auth\/(login|refresh)$/.test(req.url);

  return next(req).pipe(
    catchError((err) => {
      if (err.status === 401 && !isAuthEndpoint) {
        authService.logout();
        router.navigate(['/auth/login'], { queryParams: { returnUrl: router.url } });
      }
      return throwError(() => err);
    })
  );
};
