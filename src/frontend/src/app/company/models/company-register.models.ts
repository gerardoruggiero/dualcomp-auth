import { BaseClass, BaseTypeClass } from '../../shared/models/BaseType';

// DTOs para el registro de empresa (basados en el backend)
export interface RegisterCompanyAddressDto {
  addressTypeId: string;
  address: string;
  isPrimary: boolean;
}

export interface RegisterCompanyEmailDto {
  emailTypeId: string;
  email: string;
  isPrimary: boolean;
}

export interface RegisterCompanyPhoneDto {
  phoneTypeId: string;
  phone: string;
  isPrimary: boolean;
}

export interface RegisterCompanySocialMediaDto {
  socialMediaTypeId: string;
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
  id: string;
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
  id?: string; // ID para elementos existentes
  addressType: string;
  address: string;
  isPrimary: boolean;
  addressTypeOptions: BaseTypeClass[];
}

export interface CompanyEmailForm {
  id?: string; // ID para elementos existentes
  emailType: string;
  email: string;
  isPrimary: boolean;
  emailTypeOptions: BaseTypeClass[];
}

export interface CompanyPhoneForm {
  id?: string; // ID para elementos existentes
  phoneType: string;
  phone: string;
  isPrimary: boolean;
  phoneTypeOptions: BaseTypeClass[];
}

export interface CompanySocialMediaForm {
  id?: string; // ID para elementos existentes
  socialMediaType: string;
  url: string;
  isPrimary: boolean;
  socialMediaTypeOptions: BaseTypeClass[];
}

export interface CompanyEmployeeForm {
  id?: string; // ID para elementos existentes
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

// ===== INTERFACES PARA EDICIÓN =====

// DTOs para actualización de empresa
export interface UpdateCompanyAddressDto {
  id?: string;
  addressTypeId: string;
  address: string;
  isPrimary: boolean;
}

export interface UpdateCompanyEmailDto {
  id?: string;
  emailTypeId: string;
  email: string;
  isPrimary: boolean;
}

export interface UpdateCompanyPhoneDto {
  id?: string;
  phoneTypeId: string;
  phone: string;
  isPrimary: boolean;
}

export interface UpdateCompanySocialMediaDto {
  id?: string;
  socialMediaTypeId: string;
  url: string;
  isPrimary: boolean;
}

export interface UpdateCompanyEmployeeDto {
  id?: string;
  fullName: string;
  email: string;
  phone?: string;
  position?: string;
  hireDate?: Date;
}

export interface UpdateCompanyCommand {
  id: string;
  name: string;
  taxId: string;
  addresses: UpdateCompanyAddressDto[];
  emails: UpdateCompanyEmailDto[];
  phones: UpdateCompanyPhoneDto[];
  socialMedias: UpdateCompanySocialMediaDto[];
  employees: UpdateCompanyEmployeeDto[];
}

// Resultados de actualización
export interface UpdateCompanyResult {
  id: string;
  name: string;
  taxId: string;
  addresses: CompanyAddressResult[];
  emails: CompanyEmailResult[];
  phones: CompanyPhoneResult[];
  socialMedias: CompanySocialMediaResult[];
  employees: CompanyEmployeeResult[];
}

// Resultados de obtención de empresa
export interface GetCompanyResult {
  id: string;
  name: string;
  taxId: string;
  addresses: CompanyAddressResult[];
  emails: CompanyEmailResult[];
  phones: CompanyPhoneResult[];
  socialMedias: CompanySocialMediaResult[];
  employees: CompanyEmployeeResult[];
}

// Resultados de listado de empresas
export interface GetCompaniesResult {
  companies: GetCompanyResult[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

