import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  PhoneTypeEntity,
  CreatePhoneTypeCommand,
  UpdatePhoneTypeCommand,
  PhoneTypeResult,
  PhoneTypeListResult
} from '../models/phone-type.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PhoneTypeService extends BaseTypeService<
  PhoneTypeEntity,
  CreatePhoneTypeCommand,
  UpdatePhoneTypeCommand,
  PhoneTypeResult,
  PhoneTypeListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/phonetypes`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): PhoneTypeEntity[] {
    console.log('PhoneTypeService - Respuesta del backend:', response);
    
    if (!response || !response.phoneTypes) {
      console.warn('PhoneTypeService - Respuesta vacía o sin phoneTypes:', response);
      return [];
    }
    
    const mappedItems = response.phoneTypes.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
    
    console.log('PhoneTypeService - Items mapeados:', mappedItems);
    return mappedItems;
  }
}
