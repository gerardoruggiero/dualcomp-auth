import { inject } from '@angular/core';
import {
  HttpInterceptorFn,
  HttpRequest,
  HttpHandlerFn,
  HttpEvent,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, catchError, switchMap, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { AuthService } from './AuthService';
import { environment } from '../../environments/environment';

export const authInterceptor: HttpInterceptorFn = (req: HttpRequest<any>, next: HttpHandlerFn): Observable<HttpEvent<any>> => {
  const authService = inject(AuthService);
  const http = inject(HttpClient);

  const token = authService.getToken();

  // URLs que NO requieren autenticación
  const publicUrls = [
    '/addresstypes',
    '/emailtypes', 
    '/phonetypes',
    '/socialmediatypes'
  ];

  // Verificar si la URL es pública
  let isPublicUrl = publicUrls.some(url => req.url.includes(url));
  
  // Solo el POST a /companies (registro) es público, PUT y GET requieren autenticación
  if (req.url.includes('/companies') && req.method === 'POST') {
    isPublicUrl = true;
  }

  // Agregar token solo si no es una URL pública
  const cloned = (token && !isPublicUrl)
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(cloned).pipe(
    catchError((error: HttpErrorResponse) => {
      // No intentar refresh token para requests de logout o URLs públicas
      if (error.status === 401 && !req.url.includes('/auth/logout') && !isPublicUrl) {
        const refreshToken = authService.getRefreshToken();
        if (!refreshToken) {
          // Solo limpiar datos locales, no llamar a logout para evitar bucles
          authService.clearLocalData();
          return throwError(() => error);
        }

        // URL correcta para refresh token
        return http.post<{ accessToken: string, refreshToken: string, expiresAt: string }>(
          `${environment.apiBaseUrl}/auth/refresh-token`, 
          { refreshToken }
        ).pipe(
          switchMap(response => {
            authService.setToken(response.accessToken, response.expiresAt);
            const retry = req.clone({ setHeaders: { Authorization: `Bearer ${response.accessToken}` } });
            return next(retry);
          }),
          catchError(err => {
            // Solo limpiar datos locales, no llamar a logout para evitar bucles
            authService.clearLocalData();
            return throwError(() => err);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
