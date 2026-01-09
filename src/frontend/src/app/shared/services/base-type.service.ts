import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
  BaseTypeEntity,
  CreateBaseTypeCommand,
  UpdateBaseTypeCommand,
  BaseTypeResult,
  BaseTypeListResult
} from '../models/base-type.models';

@Injectable({
  providedIn: 'root'
})
export abstract class BaseTypeService<
  TEntity extends BaseTypeEntity,
  TCreateCommand extends CreateBaseTypeCommand,
  TUpdateCommand extends UpdateBaseTypeCommand,
  TResult extends BaseTypeResult,
  TListResult extends BaseTypeListResult
> {
  protected http = inject(HttpClient);
  protected abstract apiUrl: string;

  // Obtener lista de tipos (sin paginación por ahora, hasta que el backend la implemente)
  getTypes(page: number = 1, pageSize: number = 10, searchTerm?: string): Observable<TListResult> {
    return this.http.get<any>(this.apiUrl).pipe(
      map(response => {
        // Mapear la respuesta del backend a la estructura esperada
        const items = this.mapBackendResponse(response);

        // Aplicar filtro de búsqueda si existe
        let filteredItems = items;
        if (searchTerm && searchTerm.trim()) {
          const term = searchTerm.trim().toLowerCase();
          filteredItems = items.filter(item =>
            item.name.toLowerCase().includes(term) ||
            (item.description && item.description.toLowerCase().includes(term))
          );
        }

        // Aplicar paginación manual
        const startIndex = (page - 1) * pageSize;
        const endIndex = startIndex + pageSize;
        const paginatedItems = filteredItems.slice(startIndex, endIndex);

        return {
          items: paginatedItems,
          totalCount: filteredItems.length,
          page: page,
          pageSize: pageSize,
          totalPages: Math.ceil(filteredItems.length / pageSize)
        } as unknown as TListResult;
      })
    );
  }

  // Método abstracto para mapear la respuesta del backend
  protected abstract mapBackendResponse(response: any): TEntity[];

  // Obtener todos los tipos (sin paginación)
  getAllTypes(): Observable<TEntity[]> {
    return this.http.get<TEntity[]>(`${this.apiUrl}/all`);
  }

  // Obtener tipo por ID
  getTypeById(id: string): Observable<TEntity> {
    return this.http.get<TEntity>(`${this.apiUrl}/${id}`);
  }

  // Crear nuevo tipo
  createType(command: TCreateCommand): Observable<TResult> {
    return this.http.post<TResult>(this.apiUrl, command);
  }

  // Actualizar tipo existente
  updateType(id: string, command: TUpdateCommand): Observable<TResult> {
    return this.http.put<TResult>(`${this.apiUrl}/${id}`, command);
  }

  // Desactivar tipo (soft delete)
  deactivateType(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  // Activar tipo
  activateType(id: string): Observable<void> {
    return this.http.patch<void>(`${this.apiUrl}/${id}/activate`, {});
  }
}
