import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

// ref: https://medium.com/@amalalbert238/write-an-authinterceptor-the-right-way-a4cb0a7ce19a
// This attaches the JWT to every outgoing request automatically
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = inject(AuthService).token();
  if (token) {
    req = req.clone({ setHeaders: { Authorization: `Bearer ${token}` } });
  }
  return next(req);
};
