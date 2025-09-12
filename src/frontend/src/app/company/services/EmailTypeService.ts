import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { BaseTypeClass } from '../../shared/models/BaseType';
import { environment } from '../../environments/environment';

interface EmailTypeResponse {
  emailTypes: Array<{ value: string }>;
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
        return response.emailTypes.map((item, index) => ({
          id: item.value, // Usar el nombre del tipo como ID
          name: item.value
        }));
      })
    );
  }
}

