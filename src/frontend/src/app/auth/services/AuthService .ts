import { Injectable, signal, inject } from '@angular/core';
import { Router } from '@angular/router';
import { UserInfo, LoginService } from './LoginService';

@Injectable({ providedIn: 'root' })
export class AuthService {
  // Usar sessionStorage para máxima seguridad (se elimina al cerrar navegador)
  private tokenKey = 'auth_token';
  private refreshTokenKey = 'refresh_token';
  private userKey = 'user_info';
  private expiresAtKey = 'token_expires_at';
  private isLoggingOut = false; // Bandera para evitar múltiples llamadas a logout
  private isRedirecting = false; // Bandera para evitar múltiples redirecciones

  private isLoggedInSignal = signal(this.hasValidToken());
  private loginService = inject(LoginService);
  private router = inject(Router);

  isLoggedIn = this.isLoggedInSignal.asReadonly();

  login(accessToken: string, refreshToken: string, expiresAt: string, user: UserInfo) {
    // Resetear banderas
    this.isLoggingOut = false;
    this.isRedirecting = false;
    // Usar sessionStorage para tokens (más seguro)
    sessionStorage.setItem(this.tokenKey, accessToken);
    sessionStorage.setItem(this.refreshTokenKey, refreshToken);
    sessionStorage.setItem(this.expiresAtKey, expiresAt);
    sessionStorage.setItem(this.userKey, JSON.stringify(user));
    
    this.isLoggedInSignal.set(true);
  }

  logout() {
    if (this.isLoggingOut) {
      return;
    }
    
    this.isLoggingOut = true;
    
    // Llamar al backend para cerrar la sesión
    this.loginService.logout().subscribe({
      next: (response) => {
        // Limpiar datos locales después del logout exitoso
        this.clearLocalData();
        // Redirigir usando múltiples métodos para asegurar que funcione
        this.forceRedirectToLogin();
      },
      error: (error) => {
        // Limpiar datos locales aunque falle el backend
        this.clearLocalData();
        // Redirigir usando múltiples métodos para asegurar que funcione
        this.forceRedirectToLogin();
      }
    });
  }

  clearLocalData() {
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.refreshTokenKey);
    sessionStorage.removeItem(this.userKey);
    sessionStorage.removeItem(this.expiresAtKey);
    this.isLoggedInSignal.set(false);
  }

  forceRedirectToLogin() {
    if (this.isRedirecting) {
      return;
    }
    
    this.isRedirecting = true;
    
    // Usar setTimeout para asegurar que la limpieza de datos se complete primero
    setTimeout(() => {
      // Usar router.navigate primero (más limpio para SPA)
      this.router.navigate(['/login']).then(() => {
        // Redirección exitosa
      }).catch(error => {
        // Fallback: window.location
        try {
          window.location.href = '/login';
        } catch (error2) {
          // Error en redirección
        }
      });
    }, 100); // Aumentar el delay para asegurar que la limpieza se complete
  }

  getToken(): string | null {
    if (!this.hasValidToken()) {
      return null;
    }
    return sessionStorage.getItem(this.tokenKey);
  }

  setToken(token: string, expiresAt: string) {
    sessionStorage.setItem(this.tokenKey, token);
    sessionStorage.setItem(this.expiresAtKey, expiresAt);
  }

  getRefreshToken(): string | null {
    return sessionStorage.getItem(this.refreshTokenKey);
  }

  getUserInfo(): UserInfo | null {
    const userStr = sessionStorage.getItem(this.userKey);
    return userStr ? JSON.parse(userStr) : null;
  }

  private hasValidToken(): boolean {
    const token = sessionStorage.getItem(this.tokenKey);
    const expiresAt = sessionStorage.getItem(this.expiresAtKey);
    
    if (!token || !expiresAt) {
      return false;
    }

    // Verificar si el token ha expirado
    const expirationDate = new Date(expiresAt);
    const now = new Date();
    
    if (now >= expirationDate) {
      // Token expirado, limpiar datos
      this.clearLocalData();
      return false;
    }

    return true;
  }

  isTokenExpired(): boolean {
    const expiresAt = sessionStorage.getItem(this.expiresAtKey);
    if (!expiresAt) return true;
    
    const expirationDate = new Date(expiresAt);
    const now = new Date();
    return now >= expirationDate;
  }
}
