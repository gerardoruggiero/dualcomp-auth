import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  DocumentTypeEntity,
  CreateDocumentTypeCommand,
  UpdateDocumentTypeCommand,
  DocumentTypeResult,
  DocumentTypeListResult
} from '../models/document-type.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DocumentTypeService extends BaseTypeService<
  DocumentTypeEntity,
  CreateDocumentTypeCommand,
  UpdateDocumentTypeCommand,
  DocumentTypeResult,
  DocumentTypeListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/documenttypes`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): DocumentTypeEntity[] {
    console.log('DocumentTypeService - Respuesta del backend:', response);
    
    if (!response || !response.documentTypes) {
      console.warn('DocumentTypeService - Respuesta vacía o sin documentTypes:', response);
      return [];
    }
    
    const mappedItems = response.documentTypes.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
    
    console.log('DocumentTypeService - Items mapeados:', mappedItems);
    return mappedItems;
  }
}

