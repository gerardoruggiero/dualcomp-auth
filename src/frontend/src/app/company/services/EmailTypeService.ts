import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { BaseTypeClass } from '../../shared/models/BaseType';
import { environment } from '../../environments/environment';

interface EmailTypeResponse {
  emailTypes: Array<{ id: string; value: string; name?: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class EmailTypeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/emailtypes`;

  getAll(): Observable<BaseTypeClass[]> {
    return this.http.get<EmailTypeResponse>(this.apiUrl).pipe(
      map(response => {
        console.log('EmailTypes response:', response);
        return response.emailTypes.map(item => ({
          id: item.id, // Usar el ID real del backend
          name: item.name || item.value // Usar name si existe, sino value
        }));
      })
    );
  }
}

