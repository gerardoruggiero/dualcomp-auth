import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  EmailTypeEntity,
  CreateEmailTypeCommand,
  UpdateEmailTypeCommand,
  EmailTypeResult,
  EmailTypeListResult
} from '../models/email-type.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class EmailTypeService extends BaseTypeService<
  EmailTypeEntity,
  CreateEmailTypeCommand,
  UpdateEmailTypeCommand,
  EmailTypeResult,
  EmailTypeListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/emailtypes`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): EmailTypeEntity[] {
    if (!response || !response.emailTypes) {
      return [];
    }
    
    return response.emailTypes.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
  }
}
