import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { BaseTypeClass } from '../../shared/models/BaseType';
import { environment } from '../../environments/environment';

interface SocialMediaTypeResponse {
  socialMediaTypes: Array<{ value: string }>;
}

@Injectable({
  providedIn: 'root'
})
export class SocialMediaTypeService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiBaseUrl}/socialmediatypes`;

  getAll(): Observable<BaseTypeClass[]> {
    return this.http.get<SocialMediaTypeResponse>(this.apiUrl).pipe(
      map(response => {
        console.log('SocialMediaTypes response:', response);
        return response.socialMediaTypes.map((item, index) => ({
          id: item.value, // Usar el nombre del tipo como ID
          name: item.value
        }));
      })
    );
  }
}

