import { Injectable } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from './AuthService';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // Verificar si hay un token válido (no expirado)
  if (auth.getToken() && !auth.isTokenExpired()) {
    return true;
  }

  // Si no hay token válido, solo limpiar datos locales y redirigir
  // No llamar a auth.logout() para evitar bucles
  auth.clearLocalData();
  // Redirigir usando múltiples métodos para asegurar que funcione
  auth.forceRedirectToLogin();
  return false;
};
