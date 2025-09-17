import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseTypeService } from './base-type.service';
import { 
  SocialMediaTypeEntity,
  CreateSocialMediaTypeCommand,
  UpdateSocialMediaTypeCommand,
  SocialMediaTypeResult,
  SocialMediaTypeListResult
} from '../models/social-media-type.models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SocialMediaTypeService extends BaseTypeService<
  SocialMediaTypeEntity,
  CreateSocialMediaTypeCommand,
  UpdateSocialMediaTypeCommand,
  SocialMediaTypeResult,
  SocialMediaTypeListResult
> {
  protected apiUrl = `${environment.apiBaseUrl}/socialmediatypes`;

  // Mapear la respuesta del backend
  protected mapBackendResponse(response: any): SocialMediaTypeEntity[] {
    if (!response || !response.socialMediaTypes) {
      return [];
    }
    
    return response.socialMediaTypes.map((item: any) => ({
      id: item.id,
      name: item.name || item.value, // Usar name si existe, sino value
      description: item.description || '', // Usar description del backend si existe
      isActive: item.isActive !== undefined ? item.isActive : true // Usar isActive del backend si existe
    }));
  }
}
