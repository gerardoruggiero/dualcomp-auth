import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit, OnChanges, SimpleChanges, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaseTypeEntity, CreateBaseTypeCommand, UpdateBaseTypeCommand } from '../../models/base-type.models';

export interface TypeModalConfig {
  title: string;
  nameLabel: string;
  descriptionLabel: string;
  namePlaceholder: string;
  descriptionPlaceholder: string;
  saveButtonText: string;
  cancelButtonText: string;
  saveButtonClass: string;
}

@Component({
  selector: 'app-type-modal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="modal fade" [class.show]="isVisible()" [style.display]="isVisible() ? 'block' : 'none'" tabindex="-1" role="dialog" [attr.aria-hidden]="!isVisible()">
      <div class="modal-dialog modal-lg" role="document">
        <div class="modal-content">
          <div class="modal-header">
            <h4 class="modal-title">
              <i class="fas fa-{{ isEditMode() ? 'edit' : 'plus' }}"></i>
              {{ config.title }}
            </h4>
            <button type="button" class="close" (click)="onCancel()" aria-label="Close">
              <span aria-hidden="true">&times;</span>
            </button>
          </div>
          
          <form (ngSubmit)="onSave()" #typeForm="ngForm">
            <div class="modal-body">
              <div class="form-group">
                <label for="name">{{ config.nameLabel }} <span class="text-danger">*</span></label>
                <input 
                  type="text" 
                  id="name"
                  name="name"
                  class="form-control" 
                  [(ngModel)]="formData().name"
                  [placeholder]="config.namePlaceholder"
                  required
                  maxlength="50"
                  [class.is-invalid]="nameError()"
                  #nameInput="ngModel">
                <div class="invalid-feedback" *ngIf="nameError()">
                  {{ nameError() }}
                </div>
              </div>
              
              <div class="form-group">
                <label for="description">{{ config.descriptionLabel }}</label>
                <textarea 
                  id="description"
                  name="description"
                  class="form-control" 
                  [(ngModel)]="formData().description"
                  [placeholder]="config.descriptionPlaceholder"
                  maxlength="200"
                  rows="3"></textarea>
                <small class="form-text text-muted">
                  {{ (formData().description.length || 0) }}/200 caracteres
                </small>
              </div>
              
              <div class="form-group" *ngIf="isEditMode()">
                <div class="form-check">
                  <input 
                    type="checkbox" 
                    id="isActive"
                    name="isActive"
                    class="form-check-input" 
                    [(ngModel)]="formData().isActive">
                  <label class="form-check-label" for="isActive">
                    Activo
                  </label>
                </div>
                <small class="form-text text-muted">
                  Desmarcar para desactivar este tipo
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
                {{ config.cancelButtonText }}
              </button>
              <button 
                type="submit" 
                class="btn {{ config.saveButtonClass }}" 
                [disabled]="!typeForm.form.valid || isLoading()">
                <i class="fas fa-spinner fa-spin" *ngIf="isLoading()"></i>
                <i class="fas fa-{{ isEditMode() ? 'save' : 'plus' }}" *ngIf="!isLoading()"></i>
                {{ config.saveButtonText }}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
    
    <div class="modal-backdrop fade" [class.show]="isVisible()" [style.display]="isVisible() ? 'block' : 'none'"></div>
  `,
  styleUrls: ['./type-modal.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TypeModalComponent implements OnInit, OnChanges {
  @Input() isVisible = signal(false);
  @Input() config: TypeModalConfig = this.getDefaultConfig();
  @Input() isLoading = signal(false);
  @Input() initialData: BaseTypeEntity | null = null;

  @Output() onSaveEvent = new EventEmitter<CreateBaseTypeCommand | UpdateBaseTypeCommand>();
  @Output() onCancelEvent = new EventEmitter<void>();

  // Estado del formulario
  formData = signal({
    name: '',
    description: '',
    isActive: true
  });

  // Estado de validación
  nameError = signal<string | null>(null);

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
        name: this.initialData.name,
        description: this.initialData.description || '',
        isActive: this.initialData.isActive
      });
    } else {
      this.formData.set({
        name: '',
        description: '',
        isActive: true
      });
    }
    this.nameError.set(null);
  }

  // Validar formulario
  validateForm(): boolean {
    this.nameError.set(null);

    if (!this.formData().name?.trim()) {
      this.nameError.set('El nombre es requerido');
      return false;
    }

    if (this.formData().name.length > 50) {
      this.nameError.set('El nombre no puede exceder 50 caracteres');
      return false;
    }

    return true;
  }

  // Manejar guardado
  onSave() {
    if (!this.validateForm()) {
      return;
    }

    const formData = this.formData();
    
    if (this.isEditMode()) {
      const updateCommand: UpdateBaseTypeCommand = {
        id: this.initialData!.id,
        name: formData.name.trim(),
        description: formData.description?.trim() || undefined,
        isActive: formData.isActive
      };
      this.onSaveEvent.emit(updateCommand);
    } else {
      const createCommand: CreateBaseTypeCommand = {
        name: formData.name.trim(),
        description: formData.description?.trim() || undefined
      };
      this.onSaveEvent.emit(createCommand);
    }
  }

  // Manejar cancelación
  onCancel() {
    this.resetForm();
    this.onCancelEvent.emit();
  }

  // Configuración por defecto
  private getDefaultConfig(): TypeModalConfig {
    return {
      title: 'Tipo',
      nameLabel: 'Nombre',
      descriptionLabel: 'Descripción',
      namePlaceholder: 'Ingrese el nombre',
      descriptionPlaceholder: 'Ingrese una descripción (opcional)',
      saveButtonText: 'Guardar',
      cancelButtonText: 'Cancelar',
      saveButtonClass: 'btn-primary'
    };
  }
}
