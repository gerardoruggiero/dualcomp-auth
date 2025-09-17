import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-basic-info-section',
  imports: [CommonModule, FormsModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3 class="card-title">
          <i class="fas fa-building"></i>
          Datos Básicos de la Empresa
        </h3>
        <div class="card-tools">
          <button type="button" class="btn btn-tool" (click)="toggleSection()">
            <i class="fas fa-minus" *ngIf="!isCollapsed"></i>
            <i class="fas fa-plus" *ngIf="isCollapsed"></i>
          </button>
        </div>
      </div>
      
      <div class="card-body" [class.collapse]="isCollapsed" [class.show]="!isCollapsed">
        <div class="row">
          <div class="col-md-6">
            <div class="form-group">
              <label for="companyName">Nombre de la Empresa *</label>
              <input 
                type="text" 
                id="companyName"
                class="form-control" 
                [ngModel]="companyName" 
                (ngModelChange)="onCompanyNameChange($event)"
                name="companyName"
                placeholder="Ingrese el nombre de la empresa"
                required>
            </div>
          </div>
          <div class="col-md-6">
            <div class="form-group">
              <label for="taxId">Tax ID *</label>
              <input 
                type="text" 
                id="taxId"
                class="form-control" 
                [ngModel]="taxId" 
                (ngModelChange)="onTaxIdChange($event)"
                name="taxId"
                placeholder="Ingrese el Tax ID"
                required>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class BasicInfoSectionComponent {
  @Input() companyName: string = '';
  @Input() taxId: string = '';
  @Input() isCollapsed: boolean = false;
  
  @Output() companyNameChange = new EventEmitter<string>();
  @Output() taxIdChange = new EventEmitter<string>();
  @Output() toggleSectionEvent = new EventEmitter<void>();

  onCompanyNameChange(value: string) {
    this.companyName = value;
    this.companyNameChange.emit(value);
  }

  onTaxIdChange(value: string) {
    this.taxId = value;
    this.taxIdChange.emit(value);
  }

  toggleSection() {
    this.toggleSectionEvent.emit();
  }
}
