import { Injectable, signal } from '@angular/core';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
import {
  CompanyRegisterForm,
  CompanyAddressForm,
  CompanyEmailForm,
  CompanyPhoneForm,
  CompanySocialMediaForm,
  CompanyEmployeeForm
} from '../../models/company-register.models';
import { AddressTypeService } from '../../services/AddressTypeService';
import { EmailTypeService } from '../../services/EmailTypeService';
import { PhoneTypeService } from '../../services/PhoneTypeService';
import { SocialMediaTypeService } from '../../services/SocialMediaTypeService';
import { ModuloService } from '../../../shared/services/modulo.service';
import { BaseTypeClass } from '../../../shared/models/BaseType';

@Injectable({
  providedIn: 'root'
})
export class CompanyFormService {
  constructor(
    private addressTypeService: AddressTypeService,
    private emailTypeService: EmailTypeService,
    private phoneTypeService: PhoneTypeService,
    private socialMediaTypeService: SocialMediaTypeService,
    private moduloService: ModuloService
  ) { }

  // Estado de secciones colapsables
  sectionsCollapsed = signal<{
    basicInfo: boolean;
    addresses: boolean;
    emails: boolean;
    phones: boolean;
    socialMedias: boolean;
    employees: boolean;
    modules: boolean;
  }>({
    basicInfo: false,
    addresses: true,
    emails: true,
    phones: true,
    socialMedias: true,
    employees: true,
    modules: true
  });

  // Cargar todas las opciones de tipos
  loadTypeOptions() {
    return forkJoin({
      addressTypes: this.addressTypeService.getAll(),
      emailTypes: this.emailTypeService.getAll(),
      phoneTypes: this.phoneTypeService.getAll(),
      socialMediaTypes: this.socialMediaTypeService.getAll(),
      modules: this.moduloService.getTypes().pipe(map(r => r.items))
    });
  }

  // Crear formulario vacío
  createEmptyForm(typeOptions: any): CompanyRegisterForm {
    const addresses: CompanyAddressForm[] = [{
      addressType: '',
      address: '',
      isPrimary: true,
      addressTypeOptions: typeOptions.addressTypes || []
    }];

    const emails: CompanyEmailForm[] = [{
      emailType: '',
      email: '',
      isPrimary: true,
      emailTypeOptions: typeOptions.emailTypes || []
    }];

    const phones: CompanyPhoneForm[] = [{
      phoneType: '',
      phone: '',
      isPrimary: true,
      phoneTypeOptions: typeOptions.phoneTypes || []
    }];

    const socialMedias: CompanySocialMediaForm[] = [{
      socialMediaType: '',
      url: '',
      isPrimary: true,
      socialMediaTypeOptions: typeOptions.socialMediaTypes || []
    }];

    const employees: CompanyEmployeeForm[] = [{
      fullName: '',
      email: '',
      phone: '',
      position: '',
      hireDate: new Date()
    }];

    return {
      name: '',
      taxId: '',
      addresses,
      emails,
      phones,
      socialMedias,
      employees,
      addressTypeOptions: typeOptions.addressTypes || [],
      emailTypeOptions: typeOptions.emailTypes || [],
      phoneTypeOptions: typeOptions.phoneTypes || [],
      socialMediaTypeOptions: typeOptions.socialMediaTypes || [],
      moduleOptions: typeOptions.modules || [],
      selectedModuleIds: []
    };
  }

  // Métodos para agregar elementos
  addAddress(form: CompanyRegisterForm): CompanyAddressForm {
    return {
      addressType: '',
      address: '',
      isPrimary: form.addresses.length === 0,
      addressTypeOptions: form.addressTypeOptions
    };
  }

  addEmail(form: CompanyRegisterForm): CompanyEmailForm {
    return {
      emailType: '',
      email: '',
      isPrimary: form.emails.length === 0,
      emailTypeOptions: form.emailTypeOptions
    };
  }

  addPhone(form: CompanyRegisterForm): CompanyPhoneForm {
    return {
      phoneType: '',
      phone: '',
      isPrimary: form.phones.length === 0,
      phoneTypeOptions: form.phoneTypeOptions
    };
  }

  addSocialMedia(form: CompanyRegisterForm): CompanySocialMediaForm {
    return {
      socialMediaType: '',
      url: '',
      isPrimary: form.socialMedias.length === 0,
      socialMediaTypeOptions: form.socialMediaTypeOptions
    };
  }

  addEmployee(): CompanyEmployeeForm {
    return {
      fullName: '',
      email: '',
      phone: '',
      position: '',
      hireDate: new Date()
    };
  }

  // Métodos para remover elementos
  removeAddress(form: CompanyRegisterForm, index: number): CompanyRegisterForm {
    return {
      ...form,
      addresses: form.addresses.filter((_, i) => i !== index)
    };
  }

  removeEmail(form: CompanyRegisterForm, index: number): CompanyRegisterForm {
    return {
      ...form,
      emails: form.emails.filter((_, i) => i !== index)
    };
  }

  removePhone(form: CompanyRegisterForm, index: number): CompanyRegisterForm {
    return {
      ...form,
      phones: form.phones.filter((_, i) => i !== index)
    };
  }

  removeSocialMedia(form: CompanyRegisterForm, index: number): CompanyRegisterForm {
    return {
      ...form,
      socialMedias: form.socialMedias.filter((_, i) => i !== index)
    };
  }

  removeEmployee(form: CompanyRegisterForm, index: number): CompanyRegisterForm {
    return {
      ...form,
      employees: form.employees.filter((_, i) => i !== index)
    };
  }

  // Toggle de secciones
  toggleSection(section: 'basicInfo' | 'addresses' | 'emails' | 'phones' | 'socialMedias' | 'employees' | 'modules') {
    this.sectionsCollapsed.update(current => ({
      ...current,
      [section]: !current[section]
    }));
  }

  // Validaciones
  validateForm(form: CompanyRegisterForm): string[] {
    const errors: string[] = [];

    // Validar datos básicos
    if (!form.name.trim()) {
      errors.push('El nombre de la empresa es obligatorio');
    }
    if (!form.taxId.trim()) {
      errors.push('El Tax ID es obligatorio');
    }

    // Validar al menos un elemento de cada tipo
    if (form.addresses.length === 0) {
      errors.push('Debe agregar al menos una dirección');
    }
    if (form.emails.length === 0) {
      errors.push('Debe agregar al menos un email');
    }
    if (form.phones.length === 0) {
      errors.push('Debe agregar al menos un teléfono');
    }
    if (form.socialMedias.length === 0) {
      errors.push('Debe agregar al menos una red social');
    }
    if (form.employees.length === 0) {
      errors.push('Debe agregar al menos un empleado');
    }
    if (form.selectedModuleIds.length === 0) {
      errors.push('Debe asignar al menos un módulo a la empresa');
    }

    // Validar que todos los elementos tengan los campos requeridos
    form.addresses.forEach((addr, index) => {
      if (!addr.addressType || !addr.address.trim()) {
        errors.push(`Dirección ${index + 1}: tipo y dirección son obligatorios`);
      }
    });

    form.emails.forEach((email, index) => {
      if (!email.emailType || !email.email.trim()) {
        errors.push(`Email ${index + 1}: tipo y email son obligatorios`);
      }
      // Validar formato de email
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (email.email.trim() && !emailRegex.test(email.email.trim())) {
        errors.push(`Email ${index + 1}: formato de email inválido`);
      }
    });

    form.phones.forEach((phone, index) => {
      if (!phone.phoneType || !phone.phone.trim()) {
        errors.push(`Teléfono ${index + 1}: tipo y teléfono son obligatorios`);
      }
    });

    form.socialMedias.forEach((social, index) => {
      if (!social.socialMediaType || !social.url.trim()) {
        errors.push(`Red social ${index + 1}: tipo y URL son obligatorios`);
      }
    });

    form.employees.forEach((employee, index) => {
      if (!employee.fullName.trim() || !employee.email.trim()) {
        errors.push(`Empleado ${index + 1}: nombre completo y email son obligatorios`);
      }
      // Validar formato de email del empleado
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (employee.email.trim() && !emailRegex.test(employee.email.trim())) {
        errors.push(`Empleado ${index + 1}: formato de email inválido`);
      }
    });

    return errors;
  }
}
