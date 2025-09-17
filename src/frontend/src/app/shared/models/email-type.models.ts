import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para EmailType
export interface EmailTypeEntity extends BaseTypeEntity {}

export interface CreateEmailTypeCommand extends CreateBaseTypeCommand {}

export interface UpdateEmailTypeCommand extends UpdateBaseTypeCommand {}

export interface EmailTypeResult extends BaseTypeResult {}

export interface EmailTypeListResult extends BaseTypeListResult {}

// Formulario para EmailType
export interface EmailTypeForm {
  name: string;
  description: string;
}
