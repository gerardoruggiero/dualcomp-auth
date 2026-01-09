import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit, OnChanges, SimpleChanges, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UserEntity, CreateUserCommand, UpdateUserCommand } from '../../models/user.models';

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

              <div class="form-group">
                <div class="custom-control custom-switch">
                  <input 
                    type="checkbox" 
                    class="custom-control-input" 
                    id="isCompanyAdmin" 
                    name="isCompanyAdmin"
                    [(ngModel)]="formData().isCompanyAdmin">
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
        isCompanyAdmin: false
    });

    // Estado de validación
    firstNameError = signal<string | null>(null);
    lastNameError = signal<string | null>(null);
    emailError = signal<string | null>(null);

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
        if (this.initialData) {
            this.formData.set({
                firstName: this.initialData.firstName,
                lastName: this.initialData.lastName,
                email: this.initialData.email,
                isCompanyAdmin: this.initialData.isCompanyAdmin
            });
        } else {
            this.formData.set({
                firstName: '',
                lastName: '',
                email: '',
                isCompanyAdmin: false
            });
        }
        this.resetErrors();
    }

    resetErrors() {
        this.firstNameError.set(null);
        this.lastNameError.set(null);
        this.emailError.set(null);
    }

    // Validar formulario manualmente
    validateForm(): boolean {
        this.resetErrors();
        let isValid = true;

        if (!this.formData().firstName?.trim()) {
            this.firstNameError.set('El nombre es requerido');
            isValid = false;
        }

        if (!this.formData().lastName?.trim()) {
            this.lastNameError.set('El apellido es requerido');
            isValid = false;
        }

        if (!this.formData().email?.trim()) {
            this.emailError.set('El email es requerido');
            isValid = false;
        } else {
            // Simple regex for email validation if needed, or rely on Angular 'email' validator in template
            // HTML5 email validator is active via 'email' attribute
        }

        return isValid;
    }

    // Manejar guardado
    onSave() {
        if (!this.validateForm()) {
            return;
        }

        const formData = this.formData();

        if (this.isEditMode()) {
            const updateCommand: UpdateUserCommand = {
                id: this.initialData!.id,
                firstName: formData.firstName.trim(),
                lastName: formData.lastName.trim(),
                email: formData.email.trim(),
                isCompanyAdmin: formData.isCompanyAdmin
            };
            this.onSaveEvent.emit(updateCommand);
        } else {
            const createCommand: CreateUserCommand = {
                firstName: formData.firstName.trim(),
                lastName: formData.lastName.trim(),
                email: formData.email.trim(),
                isCompanyAdmin: formData.isCompanyAdmin
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
