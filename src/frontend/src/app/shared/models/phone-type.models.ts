import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para PhoneType
export interface PhoneTypeEntity extends BaseTypeEntity {}

export interface CreatePhoneTypeCommand extends CreateBaseTypeCommand {}

export interface UpdatePhoneTypeCommand extends UpdateBaseTypeCommand {}

export interface PhoneTypeResult extends BaseTypeResult {}

export interface PhoneTypeListResult extends BaseTypeListResult {}

// Formulario para PhoneType
export interface PhoneTypeForm {
  name: string;
  description: string;
}
