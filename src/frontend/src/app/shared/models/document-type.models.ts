import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para DocumentType
export interface DocumentTypeEntity extends BaseTypeEntity {}

export interface CreateDocumentTypeCommand extends CreateBaseTypeCommand {}

export interface UpdateDocumentTypeCommand extends UpdateBaseTypeCommand {}

export interface DocumentTypeResult extends BaseTypeResult {}

export interface DocumentTypeListResult extends BaseTypeListResult {}

// Formulario para DocumentType
export interface DocumentTypeForm {
  name: string;
  description: string;
}

