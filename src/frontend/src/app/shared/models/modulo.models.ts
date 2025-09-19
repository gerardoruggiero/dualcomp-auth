import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para Modulo
export interface ModuloEntity extends BaseTypeEntity {}

export interface CreateModuloCommand extends CreateBaseTypeCommand {}

export interface UpdateModuloCommand extends UpdateBaseTypeCommand {}

export interface ModuloResult extends BaseTypeResult {}

export interface ModuloListResult extends BaseTypeListResult {}

// Respuesta del backend
export interface ModuloResponse {
  modulos: ModuloEntity[];
}
