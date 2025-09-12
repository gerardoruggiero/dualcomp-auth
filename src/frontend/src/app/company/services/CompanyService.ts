import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RegisterCompanyCommand, RegisterCompanyResult } from '../models/company-register.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CompanyService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/companies`;

  registerCompany(command: RegisterCompanyCommand): Observable<RegisterCompanyResult> {
    return this.http.post<RegisterCompanyResult>(this.apiUrl, command);
  }
}

