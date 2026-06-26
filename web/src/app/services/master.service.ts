import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MasterService {
  constructor(private http: HttpClient) {}

  private getHeader() {
    const token = localStorage.getItem('token');
    const headers: Record<string, string> = { 'Content-Type': 'application/json' };
    if (token) headers['Authorization'] = `Bearer ${token.trim()}`;
    return { headers: new HttpHeaders(headers), withCredentials: true };
  }

  get<T>(url: string): Observable<T> {
    return this.http.get<T>(url, this.getHeader());
  }

  getWithParams<T>(url: string, params: Record<string, any>): Observable<T> {
    return this.http.get<T>(url, { ...this.getHeader(), params: new HttpParams({ fromObject: params }) });
  }

  post<T>(url: string, body: unknown): Observable<T> {
    return this.http.post<T>(url, body, this.getHeader());
  }

  put<T>(url: string, body: unknown): Observable<T> {
    return this.http.put<T>(url, body, this.getHeader());
  }

  delete<T>(url: string): Observable<T> {
    return this.http.delete<T>(url, this.getHeader());
  }
}
