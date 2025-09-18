import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  TitleEntity,
  CreateTitleCommand,
  UpdateTitleCommand,
  TitleResult,
  TitleListResult
} from '../models/title.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TitleService extends BaseTypeService<
  TitleEntity,
  CreateTitleCommand,
  UpdateTitleCommand,
  TitleResult,
  TitleListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/titles`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): TitleEntity[] {
    console.log('TitleService - Respuesta del backend:', response);
    
    if (!response || !response.titles) {
      console.warn('TitleService - Respuesta vacía o sin titles:', response);
      return [];
    }
    
    const mappedItems = response.titles.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
    
    console.log('TitleService - Items mapeados:', mappedItems);
    return mappedItems;
  }
}

