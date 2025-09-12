import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { BaseTypeClass } from '../../shared/models/BaseType';
import { environment } from '../../environments/environment';

interface AddressTypeResponse {
  addressTypes: Array<{ value: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class AddressTypeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/addresstypes`;

  getAll(): Observable<BaseTypeClass[]> {
    return this.http.get<AddressTypeResponse>(this.apiUrl).pipe(
      map(response => {
        console.log('AddressTypes response:', response);
        return response.addressTypes.map((item, index) => ({
          id: item.value, // Usar el nombre del tipo como ID
          name: item.value
        }));
      })
    );
  }
}

