import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand, BaseTypeResult, BaseTypeListResult } from './base-type.models';

// Interfaces específicas para SocialMediaType
export interface SocialMediaTypeEntity extends BaseTypeEntity {}

export interface CreateSocialMediaTypeCommand extends CreateBaseTypeCommand {}

export interface UpdateSocialMediaTypeCommand extends UpdateBaseTypeCommand {}

export interface SocialMediaTypeResult extends BaseTypeResult {}

export interface SocialMediaTypeListResult extends BaseTypeListResult {}

// Formulario para SocialMediaType
export interface SocialMediaTypeForm {
  name: string;
  description: string;
}
