import { ChangeDetectionStrategy, Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { 
  CompanyRegisterForm, 
  RegisterCompanyCommand,
  CompanyAddressForm,
  CompanyEmailForm,
  CompanyPhoneForm,
  CompanySocialMediaForm,
  CompanyEmployeeForm
} from '../models/company-register.models';
import { BaseClass } from '../../shared/models/BaseType';
import { AddressTypeService } from '../services/AddressTypeService';
import { EmailTypeService } from '../services/EmailTypeService';
import { PhoneTypeService } from '../services/PhoneTypeService';
import { SocialMediaTypeService } from '../services/SocialMediaTypeService';
import { CompanyService } from '../services/CompanyService';

@Component({
  selector: 'app-company-register',
  imports: [CommonModule, FormsModule],
  templateUrl: './company-register.component.html',
  styleUrls: ['./company-register.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyRegisterComponent implements OnInit {
  router = inject(Router);
  private addressTypeService = inject(AddressTypeService);
  private emailTypeService = inject(EmailTypeService);
  private phoneTypeService = inject(PhoneTypeService);
  private socialMediaTypeService = inject(SocialMediaTypeService);
  private companyService = inject(CompanyService);

  // Estado del componente
  isLoading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');

  // Formulario
  form = signal<CompanyRegisterForm>({
    name: '',
    taxId: '',
    addresses: [],
    emails: [],
    phones: [],
    socialMedias: [],
    employees: [],
    addressTypeOptions: [],
    emailTypeOptions: [],
    phoneTypeOptions: [],
    socialMediaTypeOptions: []
  });

  // Estado de secciones colapsables
  sectionsCollapsed = signal<{
    basicInfo: boolean;
    addresses: boolean;
    emails: boolean;
    phones: boolean;
    socialMedias: boolean;
    employees: boolean;
  }>({
    basicInfo: false,
    addresses: true,
    emails: true,
    phones: true,
    socialMedias: true,
    employees: true
  });

  ngOnInit() {
    this.loadTypeOptions();
  }

  private loadTypeOptions() {
    // Cargar todos los tipos de datos en paralelo
    forkJoin({
      addressTypes: this.addressTypeService.getAll(),
      emailTypes: this.emailTypeService.getAll(),
      phoneTypes: this.phoneTypeService.getAll(),
      socialMediaTypes: this.socialMediaTypeService.getAll()
    }).subscribe({
      next: (results) => {
        console.log('All types loaded:', results);
        
        // Actualizar el formulario con todos los datos
        this.form.update(current => ({
          ...current,
          addressTypeOptions: results.addressTypes,
          emailTypeOptions: results.emailTypes,
          phoneTypeOptions: results.phoneTypes,
          socialMediaTypeOptions: results.socialMediaTypes
        }));
        
        // Ahora que todos los datos están cargados, inicializar el formulario
        this.initializeForm();
      },
      error: (error) => {
        console.error('Error loading type options:', error);
        this.errorMessage.set('Error al cargar las opciones de tipos');
      }
    });
  }

  private initializeForm() {
    // Inicializar con al menos un elemento de cada tipo
    this.addAddress();
    this.addEmail();
    this.addPhone();
    this.addSocialMedia();
    this.addEmployee();
  }

  // Métodos para agregar elementos
  addAddress() {
    const currentForm = this.form();
    console.log('Adding address with options:', currentForm.addressTypeOptions);
    const newAddress: CompanyAddressForm = {
      addressType: '',
      address: '',
      isPrimary: currentForm.addresses.length === 0,
      addressTypeOptions: currentForm.addressTypeOptions
    };
    
    this.form.update(current => ({
      ...current,
      addresses: [...current.addresses, newAddress]
    }));
  }

  addEmail() {
    const currentForm = this.form();
    const newEmail: CompanyEmailForm = {
      emailType: '',
      email: '',
      isPrimary: currentForm.emails.length === 0,
      emailTypeOptions: currentForm.emailTypeOptions
    };
    
    this.form.update(current => ({
      ...current,
      emails: [...current.emails, newEmail]
    }));
  }

  addPhone() {
    const currentForm = this.form();
    const newPhone: CompanyPhoneForm = {
      phoneType: '',
      phone: '',
      isPrimary: currentForm.phones.length === 0,
      phoneTypeOptions: currentForm.phoneTypeOptions
    };
    
    this.form.update(current => ({
      ...current,
      phones: [...current.phones, newPhone]
    }));
  }

  addSocialMedia() {
    const currentForm = this.form();
    const newSocialMedia: CompanySocialMediaForm = {
      socialMediaType: '',
      url: '',
      isPrimary: currentForm.socialMedias.length === 0,
      socialMediaTypeOptions: currentForm.socialMediaTypeOptions
    };
    
    this.form.update(current => ({
      ...current,
      socialMedias: [...current.socialMedias, newSocialMedia]
    }));
  }

  addEmployee() {
    const newEmployee: CompanyEmployeeForm = {
      fullName: '',
      email: '',
      phone: '',
      position: '',
      hireDate: new Date()
    };
    
    this.form.update(current => ({
      ...current,
      employees: [...current.employees, newEmployee]
    }));
  }

  // Métodos para remover elementos
  removeAddress(index: number) {
    this.form.update(current => ({
      ...current,
      addresses: current.addresses.filter((_, i) => i !== index)
    }));
  }

  removeEmail(index: number) {
    this.form.update(current => ({
      ...current,
      emails: current.emails.filter((_, i) => i !== index)
    }));
  }

  removePhone(index: number) {
    this.form.update(current => ({
      ...current,
      phones: current.phones.filter((_, i) => i !== index)
    }));
  }

  removeSocialMedia(index: number) {
    this.form.update(current => ({
      ...current,
      socialMedias: current.socialMedias.filter((_, i) => i !== index)
    }));
  }

  removeEmployee(index: number) {
    this.form.update(current => ({
      ...current,
      employees: current.employees.filter((_, i) => i !== index)
    }));
  }

  // Métodos para toggle de secciones
  toggleSection(section: 'basicInfo' | 'addresses' | 'emails' | 'phones' | 'socialMedias' | 'employees') {
    this.sectionsCollapsed.update(current => ({
      ...current,
      [section]: !current[section]
    }));
  }

  // Validaciones
  private validateForm(): string[] {
    const errors: string[] = [];
    const currentForm = this.form();

    // Validar datos básicos
    if (!currentForm.name.trim()) {
      errors.push('El nombre de la empresa es obligatorio');
    }
    if (!currentForm.taxId.trim()) {
      errors.push('El Tax ID es obligatorio');
    }

    // Validar al menos un elemento de cada tipo
    if (currentForm.addresses.length === 0) {
      errors.push('Debe agregar al menos una dirección');
    }
    if (currentForm.emails.length === 0) {
      errors.push('Debe agregar al menos un email');
    }
    if (currentForm.phones.length === 0) {
      errors.push('Debe agregar al menos un teléfono');
    }
    if (currentForm.socialMedias.length === 0) {
      errors.push('Debe agregar al menos una red social');
    }
    if (currentForm.employees.length === 0) {
      errors.push('Debe agregar al menos un empleado');
    }

    // Validar que todos los elementos tengan los campos requeridos
    currentForm.addresses.forEach((addr, index) => {
      if (!addr.addressType || !addr.address.trim()) {
        errors.push(`Dirección ${index + 1}: tipo y dirección son obligatorios`);
      }
    });

    currentForm.emails.forEach((email, index) => {
      if (!email.emailType || !email.email.trim()) {
        errors.push(`Email ${index + 1}: tipo y email son obligatorios`);
      }
      // Validar formato de email
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (email.email.trim() && !emailRegex.test(email.email.trim())) {
        errors.push(`Email ${index + 1}: formato de email inválido`);
      }
    });

    currentForm.phones.forEach((phone, index) => {
      if (!phone.phoneType || !phone.phone.trim()) {
        errors.push(`Teléfono ${index + 1}: tipo y teléfono son obligatorios`);
      }
    });

    currentForm.socialMedias.forEach((social, index) => {
      if (!social.socialMediaType || !social.url.trim()) {
        errors.push(`Red social ${index + 1}: tipo y URL son obligatorios`);
      }
    });

    currentForm.employees.forEach((employee, index) => {
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

  // Envío del formulario
  onSubmit() {
    this.clearMessages();

    const validationErrors = this.validateForm();
    if (validationErrors.length > 0) {
      this.errorMessage.set(validationErrors.join('. '));
      return;
    }

    this.isLoading.set(true);

    const currentForm = this.form();
    const command: RegisterCompanyCommand = {
      name: currentForm.name.trim(),
      taxId: currentForm.taxId.trim(),
      addresses: currentForm.addresses.map(addr => ({
        addressType: addr.addressType,
        address: addr.address.trim(),
        isPrimary: addr.isPrimary
      })),
      emails: currentForm.emails.map(email => ({
        emailType: email.emailType,
        email: email.email.trim(),
        isPrimary: email.isPrimary
      })),
      phones: currentForm.phones.map(phone => ({
        phoneType: phone.phoneType,
        phone: phone.phone.trim(),
        isPrimary: phone.isPrimary
      })),
      socialMedias: currentForm.socialMedias.map(social => ({
        socialMediaType: social.socialMediaType,
        url: social.url.trim(),
        isPrimary: social.isPrimary
      })),
      employees: currentForm.employees.map(employee => ({
        fullName: employee.fullName.trim(),
        email: employee.email.trim(),
        phone: employee.phone?.trim(),
        position: employee.position?.trim(),
        hireDate: employee.hireDate
      }))
    };

    this.companyService.registerCompany(command).subscribe({
      next: (result) => {
        this.isLoading.set(false);
        this.successMessage.set('Empresa registrada exitosamente. Será redirigido al login.');
        
        // Redirigir al login después de 3 segundos
        setTimeout(() => {
          this.router.navigate(['/login']);
        }, 3000);
      },
      error: (error) => {
        this.isLoading.set(false);
        this.errorMessage.set('Error al registrar la empresa. Intente nuevamente.');
      }
    });
  }

  private clearMessages() {
    this.errorMessage.set('');
    this.successMessage.set('');
  }
}
