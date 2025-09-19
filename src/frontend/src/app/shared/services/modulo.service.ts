import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  ModuloEntity,
  CreateModuloCommand,
  UpdateModuloCommand,
  ModuloResult,
  ModuloListResult,
  ModuloResponse
} from '../models/modulo.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ModuloService extends BaseTypeService<
  ModuloEntity,
  CreateModuloCommand,
  UpdateModuloCommand,
  ModuloResult,
  ModuloListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/modulos`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): ModuloEntity[] {
    console.log('ModuloService - Respuesta del backend:', response);
    
    if (!response || !response.modulos) {
      console.warn('ModuloService - Respuesta vacía o sin modulos:', response);
      return [];
    }
    
    const mappedItems = response.modulos.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
    
    console.log('ModuloService - Items mapeados:', mappedItems);
    return mappedItems;
  }
}
