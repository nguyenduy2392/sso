import { Injectable } from '@angular/core';
import { Observable, switchMap, tap, throwError, of } from 'rxjs';
import { MasterService } from './master.service';
import { environment } from '../../environments/environment';

export interface LoginRequest {
  tenantName: string;
  userName: string;
  password: string;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  userId: string;
  userName: string;
  tenantId: string;
  tenantName: string;
}

export interface StoredUser {
  userId: string;
  userName: string;
  tenantId: string;
  tenantName: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private master: MasterService) {}

  login(data: LoginRequest): Observable<LoginResponse> {
    return this.master.post<LoginResponse>(`${environment.apiUrl}/auth/login`, data).pipe(
      switchMap((res) => {
        if (!res?.accessToken) return throwError(() => ({ error: { message: 'Đăng nhập thất bại.' } }));
        return of(res);
      }),
      tap((res) => {
        localStorage.setItem('token', res.accessToken);
        localStorage.setItem('refreshToken', res.refreshToken);
        localStorage.setItem('user', JSON.stringify({
          userId: res.userId,
          userName: res.userName,
          tenantId: res.tenantId,
          tenantName: res.tenantName,
        } satisfies StoredUser));
      })
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
  }

  getStoredUser(): StoredUser | null {
    try {
      const raw = localStorage.getItem('user');
      return raw ? (JSON.parse(raw) as StoredUser) : null;
    } catch {
      return null;
    }
  }

  isLoggedIn(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;
    try {
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')));
      return Date.now() / 1000 < payload.exp;
    } catch {
      return false;
    }
  }
}
