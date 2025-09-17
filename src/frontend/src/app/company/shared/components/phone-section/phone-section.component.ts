import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyPhoneForm } from '../../../models/company-register.models';

@Component({
  selector: 'app-phone-section',
  imports: [CommonModule, FormsModule],
  template: `
          <div *ngFor="let phone of phones; let i = index" class="contact-item">
      <div class="row">
        <div class="col-md-4">
          <div class="form-group">
            <label>Tipo de Teléfono *</label>
            <select 
              class="form-control" 
              [(ngModel)]="phone.phoneType" 
              [name]="'phoneType' + i"
              required>
              <option value="">Seleccione tipo</option>
              <option *ngFor="let type of phone.phoneTypeOptions" [value]="type.id">
                {{ type.name }}
              </option>
            </select>
          </div>
        </div>
        <div class="col-md-6">
          <div class="form-group">
            <label>Teléfono *</label>
            <input 
              type="tel" 
              class="form-control" 
              [(ngModel)]="phone.phone" 
              [name]="'phone' + i"
              placeholder="+1 (555) 123-4567"
              required>
          </div>
        </div>
        <div class="col-md-2">
          <div class="form-group">
            <label>&nbsp;</label>
            <div class="form-control-static">
              <div class="form-check">
                <input 
                  type="checkbox" 
                  class="form-check-input" 
                  [(ngModel)]="phone.isPrimary" 
                  [name]="'phonePrimary' + i"
                  id="phonePrimary{{ i }}">
                <label class="form-check-label" for="phonePrimary{{ i }}">
                  Principal
                </label>
              </div>
              <button 
                type="button" 
                class="btn btn-sm btn-danger mt-2"
                (click)="removePhone(i)"
                [disabled]="phones.length === 1">
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
export class PhoneSectionComponent {
  @Input() phones: CompanyPhoneForm[] = [];
  @Output() removePhoneEvent = new EventEmitter<number>();

  removePhone(index: number) {
    this.removePhoneEvent.emit(index);
  }
}
