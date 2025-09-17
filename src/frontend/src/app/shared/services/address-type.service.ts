import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  AddressTypeEntity,
  CreateAddressTypeCommand,
  UpdateAddressTypeCommand,
  AddressTypeResult,
  AddressTypeListResult
} from '../models/address-type.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AddressTypeService extends BaseTypeService<
  AddressTypeEntity,
  CreateAddressTypeCommand,
  UpdateAddressTypeCommand,
  AddressTypeResult,
  AddressTypeListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/addresstypes`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): AddressTypeEntity[] {
    if (!response || !response.addressTypes) {
      return [];
    }
    
    return response.addressTypes.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
  }
}
