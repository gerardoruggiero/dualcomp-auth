import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { BaseTypeClass } from '../../shared/models/BaseType';
import { environment } from '../../environments/environment';

interface PhoneTypeResponse {
  phoneTypes: Array<{ id: string; value: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class PhoneTypeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/phonetypes`;

  getAll(): Observable<BaseTypeClass[]> {
    return this.http.get<PhoneTypeResponse>(this.apiUrl).pipe(
      map(response => {
        console.log('PhoneTypes response:', response);
        return response.phoneTypes.map(item => ({
          id: item.id, // Usar el ID real del backend
          name: item.value
        }));
      })
    );
  }
}

