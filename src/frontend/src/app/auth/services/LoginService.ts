import { HttpClient, HttpErrorResponse } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { catchError, map, of } from 'rxjs';
import { Observable } from "rxjs";
import { AuthService } from "./AuthService";
import { environment } from '../../environments/environment';

// Interfaces que coinciden con la API .NET
export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserInfo {
  id: string;
  email: string;
  fullName: string;
  companyId: string | null;
  isCompanyAdmin: boolean;
}

export interface LoginResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserInfo;
}

export interface LogoutResponse {
  message: string;
}

@Injectable({providedIn: 'root'})
export class LoginService{
    private loginUrl = `${environment.apiBaseUrl}/auth/login`;
    private logoutUrl = `${environment.apiBaseUrl}/auth/logout`;
    private http = inject(HttpClient);    

    login(request: LoginRequest): Observable<LoginResponse | false> {
        return this.http.post<LoginResponse>(this.loginUrl, request).pipe(
            map((res) => res),
            catchError((error: HttpErrorResponse) => {
                return of(false as const);
            })
        );
    }

    logout(): Observable<LogoutResponse | false> {
        return this.http.post<LogoutResponse>(this.logoutUrl, {}).pipe(
            map((res) => {
                return res;
            }),
            catchError((error: HttpErrorResponse) => {
                return of(false as const);
            })
        );
    }
}