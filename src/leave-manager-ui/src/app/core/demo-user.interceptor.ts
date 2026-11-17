import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { DemoUserService } from './demo-user.service';

export const demoUserInterceptor: HttpInterceptorFn = (req, next) => {
  const user = inject(DemoUserService);
  return next(req.clone({ setHeaders: { 'X-Demo-User': user.email() } }));
};
