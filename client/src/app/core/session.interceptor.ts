import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { STAFF_GATE } from './roles';
import { SessionService } from './session.service';

export const sessionInterceptor: HttpInterceptorFn = (req, next) => {
  const session = inject(SessionService);
  const router = inject(Router);
  const token = session.token();
  const salida = token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;

  return next(salida).pipe(
    catchError((error: unknown) => {
      if (
        error instanceof HttpErrorResponse &&
        error.status === 401 &&
        !(req.method === 'POST' && req.url.endsWith('/api/session')) &&
        !req.url.endsWith('/api/account/sign-in')
      ) {
        const customer = session.role() === 'Cliente' || req.url.includes('/api/account') || req.url.includes('/api/catalog');
        session.forget();
        void router.navigateByUrl(customer ? '/cuenta' : STAFF_GATE);
      }
      return throwError(() => error);
    }),
  );
};
