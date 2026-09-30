import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { Observable, tap } from 'rxjs';

interface LoginResponse { token: string; }

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly tokenKey = 'dp_token';
  readonly token = signal<string | null>(localStorage.getItem(this.tokenKey));

  constructor(private http: HttpClient) {}

  login(email: string, password: string): Observable<LoginResponse> {
    return this.http.post<LoginResponse>('/api/auth/login', { email, password })
      .pipe(tap(res => {
        localStorage.setItem(this.tokenKey, res.token);
        this.token.set(res.token);
      }));
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.token.set(null);
  }

  get isLoggedIn(): boolean { return this.token() !== null; }

  get role(): string | null {
    const locToken = this.token();
    if (!locToken) return null;
    // JWT payload is the middle segment, base64url-encoded
    const payload = JSON.parse(atob(locToken.split('.')[1]));
    return payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ?? payload['role'] ?? null; // ref: https://medium.com/@ggrokz/role-based-login-in-angular-different-ways-to-achieve-it-57a2a63c2935
  }
}
