import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  RegisterCompanyCommand, 
  RegisterCompanyResult,
  UpdateCompanyCommand,
  UpdateCompanyResult,
  GetCompanyResult,
  GetCompaniesResult
} from '../models/company-register.models';
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

  getCompanyById(id: string): Observable<GetCompanyResult> {
    console.log('Fetching company with ID:', id);
    console.log('API URL:', `${this.apiUrl}/${id}`);
    return this.http.get<GetCompanyResult>(`${this.apiUrl}/${id}`);
  }

  updateCompany(id: string, command: UpdateCompanyCommand): Observable<UpdateCompanyResult> {
    return this.http.put<UpdateCompanyResult>(`${this.apiUrl}/${id}`, command);
  }

  getCompanies(page: number = 1, pageSize: number = 10, searchTerm?: string): Observable<GetCompaniesResult> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    
    if (searchTerm && searchTerm.trim()) {
      params = params.set('searchTerm', searchTerm.trim());
    }

    return this.http.get<GetCompaniesResult>(this.apiUrl, { params });
  }
}

