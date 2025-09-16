import { ChangeDetectionStrategy, Component, inject, signal, OnInit, effect } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { 
  CompanyRegisterForm, 
  UpdateCompanyCommand,
  GetCompanyResult,
  CompanyAddressForm,
  CompanyEmailForm,
  CompanyPhoneForm,
  CompanySocialMediaForm,
  CompanyEmployeeForm
} from '../models/company-register.models';
import { BaseClass, BaseTypeClass } from '../../shared/models/BaseType';
import { AddressTypeService } from '../services/AddressTypeService';
import { EmailTypeService } from '../services/EmailTypeService';
import { PhoneTypeService } from '../services/PhoneTypeService';
import { SocialMediaTypeService } from '../services/SocialMediaTypeService';
import { CompanyService } from '../services/CompanyService';
import { AccordionSectionComponent } from '../../admin-layout/accordion-section/accordion-section.component';
import { ContentHeaderComponent, ContentHeaderAction } from '../../shared/components/content-header/content-header.component';

@Component({
  selector: 'app-company-edit',
  imports: [CommonModule, FormsModule, AccordionSectionComponent, ContentHeaderComponent],
  templateUrl: './company-edit.component.html',
  styleUrls: ['./company-edit.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyEditComponent implements OnInit {
  router = inject(Router);
  route = inject(ActivatedRoute);
  private addressTypeService = inject(AddressTypeService);
  private emailTypeService = inject(EmailTypeService);
  private phoneTypeService = inject(PhoneTypeService);
  private socialMediaTypeService = inject(SocialMediaTypeService);
  private companyService = inject(CompanyService);

  // Estado del componente
  isLoading = signal(false);
  isInitialLoading = signal(true);
  errorMessage = signal('');
  successMessage = signal('');
  companyId = signal('');

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

  // Configuración del header
  headerTitle = 'Edición de Empresa';
  headerActions: ContentHeaderAction[] = [
    {
      label: 'Volver al listado',
      icon: 'fas fa-arrow-left',
      class: 'btn btn-secondary',
      onClick: () => this.navigateToList()
    },
    {
      label: 'Guardar',
      icon: 'fas fa-save',
      class: 'btn btn-primary',
      disabled: false, // Se actualizará dinámicamente
      onClick: () => this.onSubmit()
    }
  ];

  ngOnInit() {
    // Obtener el ID de la empresa desde la ruta
    this.route.params.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.companyId.set(id);
        this.loadCompanyData(id);
      } else {
        this.errorMessage.set('ID de empresa no válido');
        this.isInitialLoading.set(false);
      }
    });

    // Actualizar el estado de los botones cuando cambie el loading
    effect(() => {
      this.isLoading(); // Leer el signal para que el effect se ejecute cuando cambie
      this.updateHeaderActions();
    });
  }

  private updateHeaderActions() {
    const isLoading = this.isLoading();
    const isInitialLoading = this.isInitialLoading();
    
    this.headerActions = [
      {
        label: 'Volver al listado',
        icon: 'fas fa-arrow-left',
        class: 'btn btn-secondary',
        disabled: isLoading,
        onClick: () => this.navigateToList()
      },
      {
        label: isLoading ? 'Actualizando...' : 'Guardar',
        icon: isLoading ? 'fas fa-spinner fa-spin' : 'fas fa-save',
        class: 'btn btn-primary',
        disabled: isLoading || isInitialLoading,
        onClick: () => this.onSubmit()
      }
    ];
  }

  private loadCompanyData(id: string) {
    this.isInitialLoading.set(true);
    this.clearMessages();

    // Cargar datos de la empresa y tipos en paralelo
    forkJoin({
      company: this.companyService.getCompanyById(id),
      addressTypes: this.addressTypeService.getAll(),
      emailTypes: this.emailTypeService.getAll(),
      phoneTypes: this.phoneTypeService.getAll(),
      socialMediaTypes: this.socialMediaTypeService.getAll()
    }).subscribe({
      next: (results) => {
        console.log('Company data loaded:', results);
        console.log('Company object specifically:', results.company);
        console.log('Company type:', typeof results.company);
        console.log('Company keys:', Object.keys(results.company));
        
        // Verificar si el objeto company tiene datos
        if (results.company && Object.keys(results.company).length > 0) {
          // Mapear los datos de la empresa al formulario
          this.mapCompanyToForm(results.company, results);
        } else {
          console.warn('Company data is empty, creating empty form');
          // Si no hay datos, crear un formulario vacío
          this.createEmptyForm(results);
        }
        this.isInitialLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading company data:', error);
        console.log('Attempting to load types only and create empty form');
        
        // Si falla la carga de la empresa, al menos cargar los tipos
        forkJoin({
          addressTypes: this.addressTypeService.getAll(),
          emailTypes: this.emailTypeService.getAll(),
          phoneTypes: this.phoneTypeService.getAll(),
          socialMediaTypes: this.socialMediaTypeService.getAll()
        }).subscribe({
          next: (typeResults) => {
            console.log('Types loaded successfully, creating empty form');
            this.createEmptyForm(typeResults);
            this.isInitialLoading.set(false);
          },
          error: (typeError) => {
            console.error('Error loading types:', typeError);
            this.errorMessage.set('Error al cargar los datos de la empresa y tipos');
            this.isInitialLoading.set(false);
          }
        });
      }
    });
  }

  private createEmptyForm(typeOptions: any) {
    console.log('Creating empty form with types:', typeOptions);
    
    // Crear formulario vacío con al menos un elemento de cada tipo
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

    // Actualizar el formulario
    this.form.set({
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
      socialMediaTypeOptions: typeOptions.socialMediaTypes || []
    });
  }

  private mapCompanyToForm(company: GetCompanyResult, typeOptions: any) {
    console.log('Mapping company to form:', company);
    
    // Mapear direcciones (con validación)
    const addresses: CompanyAddressForm[] = (company.addresses || []).map(addr => ({
      id: addr.id, // Incluir el ID del elemento existente
      addressType: addr.addressType, // ID del tipo
      address: addr.address,
      isPrimary: addr.isPrimary,
      addressTypeOptions: typeOptions.addressTypes
    }));

    // Mapear emails (con validación)
    const emails: CompanyEmailForm[] = (company.emails || []).map(email => ({
      id: email.id, // Incluir el ID del elemento existente
      emailType: email.emailType, // ID del tipo
      email: email.email,
      isPrimary: email.isPrimary,
      emailTypeOptions: typeOptions.emailTypes
    }));

    // Mapear teléfonos (con validación)
    const phones: CompanyPhoneForm[] = (company.phones || []).map(phone => ({
      id: phone.id, // Incluir el ID del elemento existente
      phoneType: phone.phoneType, // ID del tipo
      phone: phone.phone,
      isPrimary: phone.isPrimary,
      phoneTypeOptions: typeOptions.phoneTypes
    }));

    // Mapear redes sociales (con validación)
    const socialMedias: CompanySocialMediaForm[] = (company.socialMedias || []).map(social => ({
      id: social.id, // Incluir el ID del elemento existente
      socialMediaType: social.socialMediaType, // ID del tipo
      url: social.url,
      isPrimary: social.isPrimary,
      socialMediaTypeOptions: typeOptions.socialMediaTypes
    }));

    // Mapear empleados (con validación)
    const employees: CompanyEmployeeForm[] = (company.employees || []).map(employee => ({
      id: employee.id, // Incluir el ID del elemento existente
      fullName: employee.fullName,
      email: employee.email,
      phone: employee.phone,
      position: employee.position,
      hireDate: employee.hireDate ? new Date(employee.hireDate) : undefined
    }));

    // Si no hay elementos, agregar al menos uno de cada tipo
    if (addresses.length === 0) {
      addresses.push({
        addressType: '',
        address: '',
        isPrimary: true,
        addressTypeOptions: typeOptions.addressTypes
      });
    }

    if (emails.length === 0) {
      emails.push({
        emailType: '',
        email: '',
        isPrimary: true,
        emailTypeOptions: typeOptions.emailTypes
      });
    }

    if (phones.length === 0) {
      phones.push({
        phoneType: '',
        phone: '',
        isPrimary: true,
        phoneTypeOptions: typeOptions.phoneTypes
      });
    }

    if (socialMedias.length === 0) {
      socialMedias.push({
        socialMediaType: '',
        url: '',
        isPrimary: true,
        socialMediaTypeOptions: typeOptions.socialMediaTypes
      });
    }

    if (employees.length === 0) {
      employees.push({
        fullName: '',
        email: '',
        phone: '',
        position: '',
        hireDate: new Date()
      });
    }

    // Actualizar el formulario
    this.form.set({
      name: company.name || '',
      taxId: company.taxId || '',
      addresses,
      emails,
      phones,
      socialMedias,
      employees,
      addressTypeOptions: typeOptions.addressTypes,
      emailTypeOptions: typeOptions.emailTypes,
      phoneTypeOptions: typeOptions.phoneTypes,
      socialMediaTypeOptions: typeOptions.socialMediaTypes
    });
  }

  // Métodos para agregar elementos (reutilizados de company-register)
  addAddress() {
    const currentForm = this.form();
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

  // Métodos para remover elementos (reutilizados de company-register)
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

  // Métodos para toggle de secciones (reutilizados de company-register)
  toggleSection(section: 'basicInfo' | 'addresses' | 'emails' | 'phones' | 'socialMedias' | 'employees') {
    this.sectionsCollapsed.update(current => ({
      ...current,
      [section]: !current[section]
    }));
  }

  // Validaciones (reutilizadas de company-register)
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

  // Envío del formulario (modificado para actualización)
  onSubmit() {
    this.clearMessages();

    const validationErrors = this.validateForm();
    if (validationErrors.length > 0) {
      this.errorMessage.set(validationErrors.join('. '));
      return;
    }

    this.isLoading.set(true);

    try {
      const currentForm = this.form();
      const companyId = this.companyId();
      
      const command: UpdateCompanyCommand = {
        id: companyId,
        name: currentForm.name.trim(),
        taxId: currentForm.taxId.trim(),
        addresses: currentForm.addresses.map(addr => ({
          id: addr.id, // Incluir el ID si existe (para actualizaciones)
          addressTypeId: addr.addressType,
          address: addr.address.trim(),
          isPrimary: addr.isPrimary
        })),
        emails: currentForm.emails.map(email => ({
          id: email.id, // Incluir el ID si existe (para actualizaciones)
          emailTypeId: email.emailType,
          email: email.email.trim(),
          isPrimary: email.isPrimary
        })),
        phones: currentForm.phones.map(phone => ({
          id: phone.id, // Incluir el ID si existe (para actualizaciones)
          phoneTypeId: phone.phoneType,
          phone: phone.phone.trim(),
          isPrimary: phone.isPrimary
        })),
        socialMedias: currentForm.socialMedias.map(social => ({
          id: social.id, // Incluir el ID si existe (para actualizaciones)
          socialMediaTypeId: social.socialMediaType,
          url: social.url.trim(),
          isPrimary: social.isPrimary
        })),
        employees: currentForm.employees.map(employee => ({
          id: employee.id, // Incluir el ID si existe (para actualizaciones)
          fullName: employee.fullName.trim(),
          email: employee.email.trim(),
          phone: employee.phone?.trim(),
          position: employee.position?.trim(),
          hireDate: employee.hireDate
        }))
      };

      this.companyService.updateCompany(companyId, command).subscribe({
        next: (result) => {
          this.isLoading.set(false);
          this.successMessage.set('Empresa actualizada exitosamente.');
          
          // Redirigir al listado después de 2 segundos
          setTimeout(() => {
            this.router.navigate(['/company/list']);
          }, 2000);
        },
        error: (error) => {
          this.isLoading.set(false);
          console.error('Error al actualizar empresa:', error);
          this.errorMessage.set('Error al actualizar la empresa. Intente nuevamente.');
        }
      });
    } catch (mappingError) {
      this.isLoading.set(false);
      console.error('Error al mapear tipos:', mappingError);
      this.errorMessage.set('Error al procesar los datos del formulario. Verifique que todos los tipos estén seleccionados correctamente.');
    }
  }

  private clearMessages() {
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  navigateToList() {
    this.router.navigate(['/company/list']);
  }
}
