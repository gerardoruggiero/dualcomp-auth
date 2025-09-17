import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyEmployeeForm } from '../../../models/company-register.models';

@Component({
  selector: 'app-employee-section',
  imports: [CommonModule, FormsModule],
  template: `
          <div *ngFor="let employee of employees; let i = index" class="contact-item">
      <div class="row">
        <div class="col-md-3">
          <div class="form-group">
            <label>Nombre Completo *</label>
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="employee.fullName" 
              [name]="'employeeName' + i"
              placeholder="Nombre completo"
              required>
          </div>
        </div>
        <div class="col-md-3">
          <div class="form-group">
            <label>Email *</label>
            <input 
              type="email" 
              class="form-control" 
              [(ngModel)]="employee.email" 
              [name]="'employeeEmail' + i"
              placeholder="email@ejemplo.com"
              required>
          </div>
        </div>
        <div class="col-md-2">
          <div class="form-group">
            <label>Teléfono</label>
            <input 
              type="tel" 
              class="form-control" 
              [(ngModel)]="employee.phone" 
              [name]="'employeePhone' + i"
              placeholder="+1 (555) 123-4567">
          </div>
        </div>
        <div class="col-md-2">
          <div class="form-group">
            <label>Posición</label>
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="employee.position" 
              [name]="'employeePosition' + i"
              placeholder="Cargo">
          </div>
        </div>
        <div class="col-md-2">
          <div class="form-group">
            <label>&nbsp;</label>
            <div class="form-control-static">
              <button 
                type="button" 
                class="btn btn-sm btn-danger"
                (click)="removeEmployee(i)"
                [disabled]="employees.length === 1">
                <i class="fas fa-trash"></i>
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmployeeSectionComponent {
  @Input() employees: CompanyEmployeeForm[] = [];
  @Output() removeEmployeeEvent = new EventEmitter<number>();

  removeEmployee(index: number) {
    this.removeEmployeeEvent.emit(index);
  }
}
