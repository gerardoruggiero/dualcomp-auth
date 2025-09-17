import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para AddressType
export interface AddressTypeEntity extends BaseTypeEntity {}

export interface CreateAddressTypeCommand extends CreateBaseTypeCommand {}

export interface UpdateAddressTypeCommand extends UpdateBaseTypeCommand {}

export interface AddressTypeResult extends BaseTypeResult {}

export interface AddressTypeListResult extends BaseTypeListResult {}

// Formulario para AddressType
export interface AddressTypeForm {
  name: string;
  description: string;
}
