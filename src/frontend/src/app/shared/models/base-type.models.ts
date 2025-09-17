// Interfaces base para entidades de tipos
export interface BaseTypeEntity {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

// Comando base para crear tipos
export interface CreateBaseTypeCommand {
  name: string;
  description?: string;
}

// Comando base para actualizar tipos
export interface UpdateBaseTypeCommand {
  id: string;
  name: string;
  description?: string;
  isActive?: boolean;
}

// Resultado base para operaciones de tipos
export interface BaseTypeResult {
  id: string;
  name: string;
  description?: string;
  isActive: boolean;
}

// Respuesta paginada para listas de tipos
export interface BaseTypeListResult {
  items: BaseTypeEntity[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

// Configuración para formularios de tipos
export interface BaseTypeFormConfig {
  title: string;
  nameLabel: string;
  descriptionLabel: string;
  namePlaceholder: string;
  descriptionPlaceholder: string;
  saveButtonText: string;
  cancelButtonText: string;
}
