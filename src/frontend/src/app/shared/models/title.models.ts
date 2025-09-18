import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para Title
export interface TitleEntity extends BaseTypeEntity {}

export interface CreateTitleCommand extends CreateBaseTypeCommand {}

export interface UpdateTitleCommand extends UpdateBaseTypeCommand {}

export interface TitleResult extends BaseTypeResult {}

export interface TitleListResult extends BaseTypeListResult {}

// Formulario para Title
export interface TitleForm {
  name: string;
  description: string;
}

