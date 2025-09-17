import { ChangeDetectionStrategy, Component, inject, signal, OnInit, effect } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';
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
import { CompanyService } from '../services/CompanyService';
import { CompanyFormService } from '../shared/services/company-form.service';
import { AccordionSectionComponent } from '../../admin-layout/accordion-section/accordion-section.component';
import { ContentHeaderComponent, ContentHeaderAction } from '../../shared/components/content-header/content-header.component';
import { BasicInfoSectionComponent } from '../shared/components/basic-info-section/basic-info-section.component';
import { AddressSectionComponent } from '../shared/components/address-section/address-section.component';
import { EmailSectionComponent } from '../shared/components/email-section/email-section.component';
import { PhoneSectionComponent } from '../shared/components/phone-section/phone-section.component';
import { SocialMediaSectionComponent } from '../shared/components/social-media-section/social-media-section.component';
import { EmployeeSectionComponent } from '../shared/components/employee-section/employee-section.component';

@Component({
  selector: 'app-company-edit',
  imports: [
    CommonModule, 
    FormsModule, 
    AccordionSectionComponent, 
    ContentHeaderComponent,
    BasicInfoSectionComponent,
    AddressSectionComponent,
    EmailSectionComponent,
    PhoneSectionComponent,
    SocialMediaSectionComponent,
    EmployeeSectionComponent
  ],
  templateUrl: './company-edit.component.html',
  styleUrls: ['./company-edit.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyEditComponent implements OnInit {
  router = inject(Router);
  route = inject(ActivatedRoute);
  private companyService = inject(CompanyService);
  private companyFormService = inject(CompanyFormService);

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

  // Estado de secciones colapsables (usando el servicio compartido)
  get sectionsCollapsed() {
    return this.companyFormService.sectionsCollapsed;
  }

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
      typeOptions: this.companyFormService.loadTypeOptions()
    }).subscribe({
      next: (results) => {
        console.log('Company data loaded:', results);
        console.log('Company object specifically:', results.company);
        console.log('Company type:', typeof results.company);
        console.log('Company keys:', Object.keys(results.company));
        
        // Verificar si el objeto company tiene datos
        if (results.company && Object.keys(results.company).length > 0) {
          // Mapear los datos de la empresa al formulario
          this.mapCompanyToForm(results.company, results.typeOptions);
        } else {
          console.warn('Company data is empty, creating empty form');
          // Si no hay datos, crear un formulario vacío
          this.createEmptyForm(results.typeOptions);
        }
        this.isInitialLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading company data:', error);
        console.log('Attempting to load types only and create empty form');
        
        // Si falla la carga de la empresa, al menos cargar los tipos
        this.companyFormService.loadTypeOptions().subscribe({
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
    
    // Usar el servicio compartido para crear formulario vacío
    const emptyForm = this.companyFormService.createEmptyForm(typeOptions);
    this.form.set(emptyForm);
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

  // Métodos para agregar elementos (usando el servicio compartido)
  addAddress() {
    const currentForm = this.form();
    const newAddress = this.companyFormService.addAddress(currentForm);
    this.form.update(current => ({
      ...current,
      addresses: [...current.addresses, newAddress]
    }));
  }

  addEmail() {
    const currentForm = this.form();
    const newEmail = this.companyFormService.addEmail(currentForm);
    this.form.update(current => ({
      ...current,
      emails: [...current.emails, newEmail]
    }));
  }

  addPhone() {
    const currentForm = this.form();
    const newPhone = this.companyFormService.addPhone(currentForm);
    this.form.update(current => ({
      ...current,
      phones: [...current.phones, newPhone]
    }));
  }

  addSocialMedia() {
    const currentForm = this.form();
    const newSocialMedia = this.companyFormService.addSocialMedia(currentForm);
    this.form.update(current => ({
      ...current,
      socialMedias: [...current.socialMedias, newSocialMedia]
    }));
  }

  addEmployee() {
    const newEmployee = this.companyFormService.addEmployee();
    this.form.update(current => ({
      ...current,
      employees: [...current.employees, newEmployee]
    }));
  }

  // Métodos para remover elementos (usando el servicio compartido)
  removeAddress(index: number) {
    const currentForm = this.form();
    const updatedForm = this.companyFormService.removeAddress(currentForm, index);
    this.form.set(updatedForm);
  }

  removeEmail(index: number) {
    const currentForm = this.form();
    const updatedForm = this.companyFormService.removeEmail(currentForm, index);
    this.form.set(updatedForm);
  }

  removePhone(index: number) {
    const currentForm = this.form();
    const updatedForm = this.companyFormService.removePhone(currentForm, index);
    this.form.set(updatedForm);
  }

  removeSocialMedia(index: number) {
    const currentForm = this.form();
    const updatedForm = this.companyFormService.removeSocialMedia(currentForm, index);
    this.form.set(updatedForm);
  }

  removeEmployee(index: number) {
    const currentForm = this.form();
    const updatedForm = this.companyFormService.removeEmployee(currentForm, index);
    this.form.set(updatedForm);
  }

  // Métodos para actualizar datos básicos
  updateCompanyName(value: string) {
    this.form.update(current => ({
      ...current,
      name: value
    }));
  }

  updateTaxId(value: string) {
    this.form.update(current => ({
      ...current,
      taxId: value
    }));
  }

  // Métodos para toggle de secciones (usando el servicio compartido)
  toggleSection(section: 'basicInfo' | 'addresses' | 'emails' | 'phones' | 'socialMedias' | 'employees') {
    this.companyFormService.toggleSection(section);
  }

  // Validaciones (usando el servicio compartido)
  private validateForm(): string[] {
    const currentForm = this.form();
    return this.companyFormService.validateForm(currentForm);
  }

  // Envío del formulario (modificado para actualización)
  onSubmit() {
    this.clearMessages();

    const validationErrors = this.validateForm();
    if (validationErrors.length > 0) {
      this.errorMessage.set(validationErrors.join('. '));
      return;
    }

    // Debug: Verificar estado de autenticación
    console.log('=== DEBUG AUTHENTICATION ===');
    console.log('Token exists:', !!sessionStorage.getItem('auth_token'));
    console.log('Token value:', sessionStorage.getItem('auth_token')?.substring(0, 20) + '...');
    console.log('Expires at:', sessionStorage.getItem('token_expires_at'));
    console.log('Current time:', new Date().toISOString());
    console.log('============================');

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
