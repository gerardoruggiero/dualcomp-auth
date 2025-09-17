import { ChangeDetectionStrategy, Component, inject, signal, OnInit, effect } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { 
  CompanyRegisterForm, 
  RegisterCompanyCommand
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
  selector: 'app-company-register',
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
  templateUrl: './company-register.component.html',
  styleUrls: ['./company-register.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CompanyRegisterComponent implements OnInit {
  router = inject(Router);
  private companyService = inject(CompanyService);
  private companyFormService = inject(CompanyFormService);

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

  // Estado de secciones colapsables (usando el servicio compartido)
  get sectionsCollapsed() {
    return this.companyFormService.sectionsCollapsed;
  }

  // Configuración del header
  headerTitle = 'Registro de Empresa';
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
    this.loadTypeOptions();
    
    // Actualizar el estado de los botones cuando cambie el loading
    effect(() => {
      this.isLoading(); // Leer el signal para que el effect se ejecute cuando cambie
      this.updateHeaderActions();
    });
  }

  private updateHeaderActions() {
    const isLoading = this.isLoading();
    
    this.headerActions = [
      {
        label: 'Volver al listado',
        icon: 'fas fa-arrow-left',
        class: 'btn btn-secondary',
        disabled: isLoading,
        onClick: () => this.navigateToList()
      },
      {
        label: isLoading ? 'Registrando...' : 'Guardar',
        icon: isLoading ? 'fas fa-spinner fa-spin' : 'fas fa-save',
        class: 'btn btn-primary',
        disabled: isLoading,
        onClick: () => this.onSubmit()
      }
    ];
  }

  private loadTypeOptions() {
    // Usar el servicio compartido para cargar tipos
    this.companyFormService.loadTypeOptions().subscribe({
      next: (results) => {
        console.log('All types loaded:', results);
        
        // Crear formulario vacío con los tipos cargados
        const emptyForm = this.companyFormService.createEmptyForm(results);
        this.form.set(emptyForm);
      },
      error: (error) => {
        console.error('Error loading type options:', error);
        this.errorMessage.set('Error al cargar las opciones de tipos');
      }
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

  // Envío del formulario
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
      
      const command: RegisterCompanyCommand = {
        name: currentForm.name.trim(),
        taxId: currentForm.taxId.trim(),
        addresses: currentForm.addresses.map(addr => ({
          addressTypeId: addr.addressType, // Ya es el ID directamente
          address: addr.address.trim(),
          isPrimary: addr.isPrimary
        })),
        emails: currentForm.emails.map(email => ({
          emailTypeId: email.emailType, // Ya es el ID directamente
          email: email.email.trim(),
          isPrimary: email.isPrimary
        })),
        phones: currentForm.phones.map(phone => ({
          phoneTypeId: phone.phoneType, // Ya es el ID directamente
          phone: phone.phone.trim(),
          isPrimary: phone.isPrimary
        })),
        socialMedias: currentForm.socialMedias.map(social => ({
          socialMediaTypeId: social.socialMediaType, // Ya es el ID directamente
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
          console.error('Error al registrar empresa:', error);
          this.errorMessage.set('Error al registrar la empresa. Intente nuevamente.');
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
