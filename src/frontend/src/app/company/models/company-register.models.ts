import { BaseClass, BaseTypeClass } from '../../shared/models/BaseType';

// DTOs para el registro de empresa (basados en el backend)
export interface RegisterCompanyAddressDto {
  addressType: string;
  address: string;
  isPrimary: boolean;
}

export interface RegisterCompanyEmailDto {
  emailType: string;
  email: string;
  isPrimary: boolean;
}

export interface RegisterCompanyPhoneDto {
  phoneType: string;
  phone: string;
  isPrimary: boolean;
}

export interface RegisterCompanySocialMediaDto {
  socialMediaType: string;
  url: string;
  isPrimary: boolean;
}

export interface RegisterCompanyEmployeeDto {
  fullName: string;
  email: string;
  phone?: string;
  position?: string;
  hireDate?: Date;
}

export interface RegisterCompanyCommand {
  name: string;
  taxId: string;
  addresses: RegisterCompanyAddressDto[];
  emails: RegisterCompanyEmailDto[];
  phones: RegisterCompanyPhoneDto[];
  socialMedias: RegisterCompanySocialMediaDto[];
  employees: RegisterCompanyEmployeeDto[];
}

// Resultados del registro
export interface CompanyAddressResult {
  id: string;
  addressType: string;
  address: string;
  isPrimary: boolean;
}

export interface CompanyEmailResult {
  id: string;
  emailType: string;
  email: string;
  isPrimary: boolean;
}

export interface CompanyPhoneResult {
  id: string;
  phoneType: string;
  phone: string;
  isPrimary: boolean;
}

export interface CompanySocialMediaResult {
  id: string;
  socialMediaType: string;
  url: string;
  isPrimary: boolean;
}

export interface CompanyEmployeeResult {
  id: string;
  fullName: string;
  email: string;
  phone?: string;
  position?: string;
  hireDate?: Date;
}

export interface RegisterCompanyResult {
  companyId: string;
  name: string;
  taxId: string;
  addresses: CompanyAddressResult[];
  emails: CompanyEmailResult[];
  phones: CompanyPhoneResult[];
  socialMedias: CompanySocialMediaResult[];
  employees: CompanyEmployeeResult[];
}

// Modelos para el formulario (con tipos completos)
export interface CompanyAddressForm {
  addressType: string;
  address: string;
  isPrimary: boolean;
  addressTypeOptions: BaseTypeClass[];
}

export interface CompanyEmailForm {
  emailType: string;
  email: string;
  isPrimary: boolean;
  emailTypeOptions: BaseTypeClass[];
}

export interface CompanyPhoneForm {
  phoneType: string;
  phone: string;
  isPrimary: boolean;
  phoneTypeOptions: BaseTypeClass[];
}

export interface CompanySocialMediaForm {
  socialMediaType: string;
  url: string;
  isPrimary: boolean;
  socialMediaTypeOptions: BaseTypeClass[];
}

export interface CompanyEmployeeForm {
  fullName: string;
  email: string;
  phone?: string;
  position?: string;
  hireDate?: Date;
}

export interface CompanyRegisterForm {
  // Datos básicos
  name: string;
  taxId: string;
  
  // Colecciones
  addresses: CompanyAddressForm[];
  emails: CompanyEmailForm[];
  phones: CompanyPhoneForm[];
  socialMedias: CompanySocialMediaForm[];
  employees: CompanyEmployeeForm[];
  
  // Opciones para los selects
  addressTypeOptions: BaseTypeClass[];
  emailTypeOptions: BaseTypeClass[];
  phoneTypeOptions: BaseTypeClass[];
  socialMediaTypeOptions: BaseTypeClass[];
}

