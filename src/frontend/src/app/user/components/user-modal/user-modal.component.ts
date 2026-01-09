import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit, OnChanges, SimpleChanges, signal, inject, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, of } from 'rxjs';
import { debounceTime, distinctUntilChanged, switchMap, catchError, map } from 'rxjs/operators';
import { UserEntity, CreateUserCommand, UpdateUserCommand } from '../../models/user.models';
import { AuthService } from '../../../auth/services/AuthService';
import { CompanyService } from '../../../company/services/CompanyService';
import { GetCompanyResult } from '../../../company/models/company-register.models';

@Component({
  selector: 'app-user-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal fade" [class.show]="isVisible()" [style.display]="isVisible() ? 'block' : 'none'" tabindex="-1" role="dialog" [attr.aria-hidden]="!isVisible()">
      <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h4 class="modal-title">
              <i class="fas fa-{{ isEditMode() ? 'edit' : 'user-plus' }}"></i>
              {{ isEditMode() ? 'Editar Usuario' : 'Nuevo Usuario' }}
            </h4>
            <button type="button" class="close" (click)="onCancel()" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          
          <form (ngSubmit)="onSave()" #userForm="ngForm">
            <div class="modal-body">
              <div class="row">
                <div class="col-md-6">
                  <div class="form-group">
                    <label for="firstName">Nombre <span class="text-danger">*</span></label>
                    <input 
                      type="text" 
                      id="firstName"
                      name="firstName"
                      class="form-control" 
                      [(ngModel)]="formData().firstName"
                      placeholder="Ingrese el nombre"
                      required
                      maxlength="50"
                      [class.is-invalid]="firstNameError()"
                      #firstNameInput="ngModel">
                    <div class="invalid-feedback" *ngIf="firstNameError()">
                      {{ firstNameError() }}
                    </div>
                  </div>
                </div>
                <div class="col-md-6">
                  <div class="form-group">
                    <label for="lastName">Apellido <span class="text-danger">*</span></label>
                    <input 
                      type="text" 
                      id="lastName"
                      name="lastName"
                      class="form-control" 
                      [(ngModel)]="formData().lastName"
                      placeholder="Ingrese el apellido"
                      required
                      maxlength="50"
                      [class.is-invalid]="lastNameError()"
                      #lastNameInput="ngModel">
                    <div class="invalid-feedback" *ngIf="lastNameError()">
                      {{ lastNameError() }}
                    </div>
                  </div>
                </div>
              </div>

              <div class="form-group">
                <label for="email">Email <span class="text-danger">*</span></label>
                <input 
                  type="email" 
                  id="email"
                  name="email"
                  class="form-control" 
                  [(ngModel)]="formData().email"
                  placeholder="ejemplo@correo.com"
                  required
                  email
                  maxlength="100"
                  [class.is-invalid]="emailError()"
                  #emailInput="ngModel">
                <div class="invalid-feedback" *ngIf="emailError()">
                  {{ emailError() }}
                </div>
              </div>

              <!-- Company Search (Only for Admins) -->
              <div class="form-group" *ngIf="canAssignCompany() && !formData().isCompanyAdmin">
                <label for="companySearch">Empresa <span class="text-danger">*</span></label>
                <div class="position-relative">
                  <input 
                    type="text" 
                    class="form-control" 
                    id="companySearch" 
                    name="companySearch"
                    placeholder="Buscar empresa..." 
                    autocomplete="off"
                    [value]="selectedCompanyName()"
                    (input)="onSearch($any($event.target).value)"
                    (focus)="showCompanyDropdown.set(true)"
                    (blur)="onBlurSearch()"
                    [class.is-invalid]="companyError()">
                  
                  <div class="invalid-feedback" *ngIf="companyError()">
                    {{ companyError() }}
                  </div>
                  
                  <div class="dropdown-menu w-100" [class.show]="showCompanyDropdown() && filteredCompanies().length > 0">
                    <button 
                      type="button" 
                      class="dropdown-item" 
                      *ngFor="let company of filteredCompanies()"
                      (mousedown)="selectCompany(company)">
                      {{ company.name }} <small class="text-muted">({{ company.taxId }})</small>
                    </button>
                  </div>
                  
                   <div class="dropdown-menu w-100" [class.show]="showCompanyDropdown() && filteredCompanies().length === 0 && isSearching()">
                    <span class="dropdown-item-text text-muted">Buscando...</span>
                  </div>
                  
                  <div class="dropdown-menu w-100" [class.show]="showCompanyDropdown() && filteredCompanies().length === 0 && !isSearching() && hasSearched()">
                     <span class="dropdown-item-text text-muted">No se encontraron resultados</span>
                  </div>
                </div>
                <small class="form-text text-muted">
                    Escriba para buscar una empresa.
                </small>
              </div>

              <div class="form-group">
                <div class="custom-control custom-switch">
                  <input 
                    type="checkbox" 
                    class="custom-control-input" 
                    id="isCompanyAdmin" 
                    name="isCompanyAdmin"
                    [ngModel]="formData().isCompanyAdmin"
                    (ngModelChange)="onIsCompanyAdminChange($event)">
                  <label class="custom-control-label" for="isCompanyAdmin">Es Administrador</label>
                </div>
                <small class="form-text text-muted">
                  Los administradores tienen acceso total a la configuración de la empresa.
                </small>
              </div>
            </div>
            
            <div class="modal-footer">
              <button 
                type="button" 
                class="btn btn-secondary" 
                (click)="onCancel()"
                [disabled]="isLoading()">
                <i class="fas fa-times"></i>
                Cancelar
              </button>
              <button 
                type="submit" 
                class="btn btn-primary" 
                [disabled]="!userForm.form.valid || isLoading()">
                <i class="fas fa-spinner fa-spin" *ngIf="isLoading()"></i>
                <i class="fas fa-save" *ngIf="!isLoading()"></i>
                Guardar
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
    
    <div class="modal-backdrop fade" [class.show]="isVisible()" [style.display]="isVisible() ? 'block' : 'none'"></div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserModalComponent implements OnInit, OnChanges {
  private authService = inject(AuthService);
  private companyService = inject(CompanyService);

  @Input() isVisible = signal(false);
  @Input() isLoading = signal(false);
  @Input() initialData: UserEntity | null = null;

  @Output() onSaveEvent = new EventEmitter<CreateUserCommand | UpdateUserCommand>();
  @Output() onCancelEvent = new EventEmitter<void>();

  // Estado del formulario
  formData = signal({
    firstName: '',
    lastName: '',
    email: '',
    isCompanyAdmin: false,
    companyId: ''
  });

  // Estado de validación
  firstNameError = signal<string | null>(null);
  lastNameError = signal<string | null>(null);
  emailError = signal<string | null>(null);
  companyError = signal<string | null>(null);

  // Estado de Buscador de Empresas
  private searchSubject = new Subject<string>();
  filteredCompanies = signal<GetCompanyResult[]>([]);
  showCompanyDropdown = signal(false);
  selectedCompanyName = signal('');
  isSearching = signal(false);
  hasSearched = signal(false);

  // Propiedad computada para permisos
  canAssignCompany = signal(false);

  constructor() {
    // Setup search subscription
    this.searchSubject.pipe(
      debounceTime(300),
      distinctUntilChanged(),
      switchMap(term => {
        if (!term || term.length < 2) {
          return of({ companies: [] });
        }
        this.isSearching.set(true);
        return this.companyService.getCompanies(1, 10, term).pipe(
          catchError(() => of({ companies: [] }))
        );
      })
    ).subscribe(result => {
      this.isSearching.set(false);
      this.hasSearched.set(true);
      this.filteredCompanies.set(result.companies);
    });

    // Verificar permisos al inicio
    effect(() => {
      const userInfo = this.authService.getUserInfo();
      this.canAssignCompany.set(userInfo?.isCompanyAdmin ?? false);
    }, { allowSignalWrites: true });
  }

  ngOnInit() {
    this.resetForm();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['initialData'] && !changes['initialData'].firstChange) {
      this.resetForm();
    }
  }

  // Determinar si estamos en modo edición
  isEditMode(): boolean {
    return this.initialData !== null;
  }

  // Resetear formulario
  resetForm() {
    const userInfo = this.authService.getUserInfo();
    const isAdmin = userInfo?.isCompanyAdmin ?? false;
    this.canAssignCompany.set(isAdmin);

    if (this.initialData) {
      this.formData.set({
        firstName: this.initialData.firstName,
        lastName: this.initialData.lastName,
        email: this.initialData.email,
        isCompanyAdmin: this.initialData.isCompanyAdmin,
        companyId: this.initialData.companyId
      });

      // Si es admin y hay companyId, cargar el nombre de la empresa para mostrarlo
      if (isAdmin && this.initialData.companyId) {
        this.companyService.getCompanyById(this.initialData.companyId)
          .subscribe({
            next: (company) => {
              this.selectedCompanyName.set(company.name);
            },
            error: () => {
              this.selectedCompanyName.set('');
            }
          });
      } else {
        this.selectedCompanyName.set('');
      }

    } else {
      this.formData.set({
        firstName: '',
        lastName: '',
        email: '',
        isCompanyAdmin: false,
        companyId: ''
      });
      this.selectedCompanyName.set('');
    }

    this.filteredCompanies.set([]);
    this.showCompanyDropdown.set(false);
    this.hasSearched.set(false);
    this.resetErrors();
  }

  resetErrors() {
    this.firstNameError.set(null);
    this.lastNameError.set(null);
    this.emailError.set(null);
    this.companyError.set(null);
  }

  // Métodos de búsqueda
  onSearch(term: string) {
    this.selectedCompanyName.set(term); // Actualizar input visualmente
    this.searchSubject.next(term);
    this.showCompanyDropdown.set(true);

    // Si borra el texto, limpiar ID
    if (!term) {
      this.formData.update(d => ({ ...d, companyId: '' }));
    }
  }

  selectCompany(company: GetCompanyResult) {
    // Al seleccionar empresa, el toggle de Admin debe desactivarse
    this.formData.update(d => ({ ...d, companyId: company.id, isCompanyAdmin: false }));
    this.selectedCompanyName.set(company.name);
    this.showCompanyDropdown.set(false);
    this.companyError.set(null);
  }

  onBlurSearch() {
    // Pequeño delay para permitir el click en el dropdown
    setTimeout(() => {
      this.showCompanyDropdown.set(false);

      // Validación básica: Si escribió algo pero no seleccionó (no tiene ID), limpiar o dar error?
      // Si el nombre coincide con la selección actual, todo bien.
      // Si no, forzar selección o limpiar.
      // Por simplicidad: si companyId está vacío y hay texto, es inválido o se limpia.
      const currentData = this.formData();
      if (this.selectedCompanyName() && !currentData.companyId) {
        this.companyError.set('Debe seleccionar una empresa de la lista.');
      }
    }, 200);
  }

  // Manejar cambio de toggle Admin
  onIsCompanyAdminChange(isChecked: boolean) {
    this.formData.update(d => ({ ...d, isCompanyAdmin: isChecked }));

    // Si se activa admin, limpiar selección de empresa (se hereda)
    if (isChecked) {
      this.formData.update(d => ({ ...d, companyId: '' }));
      this.selectedCompanyName.set('');
      this.companyError.set(null);
    }
  }

  // Validar formulario manualmente
  validateForm(): boolean {
    this.resetErrors();
    let isValid = true;
    const data = this.formData();

    if (!data.firstName?.trim()) {
      this.firstNameError.set('El nombre es requerido');
      isValid = false;
    }

    if (!data.lastName?.trim()) {
      this.lastNameError.set('El apellido es requerido');
      isValid = false;
    }

    if (!data.email?.trim()) {
      this.emailError.set('El email es requerido');
      isValid = false;
    }

    // Validar empresa si tiene permisos para asignar
    if (this.canAssignCompany()) {
      // Si NO es super admin, y NO tiene empresa seleccionada, es error.
      // (Si es super admin, la empresa se hereda/ignora, así que no es obligatoria aqui)
      if (!data.isCompanyAdmin && !data.companyId && !this.isEditMode()) {
        this.companyError.set('Debe seleccionar una empresa.');
        isValid = false;
      }
    }

    return isValid;
  }

  // Manejar guardado
  onSave() {
    if (!this.validateForm()) {
      return;
    }

    const formData = this.formData();
    // Si es Admin o no hay companyId seleccionado, enviamos undefined/null para que el backend aplique herencia.
    // Especialmente si isCompanyAdmin es true, ignoramos lo que haya en companyId (aunque ya lo limpiamos).
    // Convertir string vacío a undefined para evitar errores de serialización con Guid.
    const targetCompanyId = (formData.isCompanyAdmin || !formData.companyId) ? undefined : formData.companyId;

    if (this.isEditMode()) {
      const updateCommand: UpdateUserCommand = {
        id: this.initialData!.id,
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email.trim(),
        isCompanyAdmin: formData.isCompanyAdmin,
        companyId: targetCompanyId as string // Casting as string to satisfy stricter TS if any, but backend receives it fine if interface allows optional
      };

      this.onSaveEvent.emit(updateCommand);
    } else {
      const createCommand: CreateUserCommand = {
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email.trim(),
        isCompanyAdmin: formData.isCompanyAdmin,
        companyId: targetCompanyId as string
      };
      this.onSaveEvent.emit(createCommand);
    }
  }

  // Manejar cancelación
  onCancel() {
    this.resetForm();
    this.onCancelEvent.emit();
  }
}
