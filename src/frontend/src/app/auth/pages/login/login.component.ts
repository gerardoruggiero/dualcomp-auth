import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { LoginService, LoginRequest } from '../../services/LoginService';
import { AuthService } from '../../services/AuthService';

@Component({
  selector: 'app-login',
  imports: [RouterModule],
  templateUrl: './login.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginComponent { 
  
  email = signal('');
  password = signal('');
  errorMessage = signal('');
  isLoading = signal(false);

  loginService = inject(LoginService);
  authService = inject(AuthService);
  router = inject(Router);

  constructor() {
    // Siempre verificar si hay token válido al cargar el componente
    if (this.authService.getToken() && !this.authService.isTokenExpired()) {
      this.router.navigate(['/dashboard']);
    }
  }

  clearError() {
    this.errorMessage.set('');
  }

  onSubmit() {
    if (!this.email().trim() || !this.password().trim()) {
      this.errorMessage.set('Email y contraseña son obligatorios');
      return;
    }

    // Validar formato de email básico
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(this.email())) {
      this.errorMessage.set('Por favor ingrese un email válido');
      return;
    }

    this.isLoading.set(true);
    this.clearError();

    const loginRequest: LoginRequest = {
      email: this.email().trim(),
      password: this.password()
    };

    this.loginService.login(loginRequest).subscribe({
      next: (result) => {
        this.isLoading.set(false);
        if (result) {
          // Usar la nueva estructura de respuesta
          this.authService.login(
            result.accessToken,
            result.refreshToken,
            result.expiresAt,
            result.user
          );
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage.set('Credenciales inválidas');
        }
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set('Error al iniciar sesión. Intente nuevamente.');
      }
    });
  }
}
