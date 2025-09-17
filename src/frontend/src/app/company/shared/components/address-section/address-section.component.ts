import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyAddressForm } from '../../../models/company-register.models';

@Component({
  selector: 'app-address-section',
  imports: [CommonModule, FormsModule],
  template: `
          <div *ngFor="let address of addresses; let i = index" class="contact-item">
      <div class="row">
        <div class="col-md-4">
          <div class="form-group">
            <label>Tipo de Dirección *</label>
            <select 
              class="form-control" 
              [(ngModel)]="address.addressType" 
              [name]="'addressType' + i"
              required>
              <option value="">Seleccione tipo</option>
              <option *ngFor="let type of address.addressTypeOptions" [value]="type.id">
                {{ type.name }}
              </option>
            </select>
          </div>
        </div>
        <div class="col-md-6">
          <div class="form-group">
            <label>Dirección *</label>
            <input 
              type="text" 
              class="form-control" 
              [(ngModel)]="address.address" 
              [name]="'address' + i"
              placeholder="Ingrese la dirección completa"
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
                  [(ngModel)]="address.isPrimary" 
                  [name]="'addressPrimary' + i"
                  id="addressPrimary{{ i }}">
                <label class="form-check-label" for="addressPrimary{{ i }}">
                  Principal
                </label>
              </div>
              <button 
                type="button" 
                class="btn btn-sm btn-danger mt-2"
                (click)="removeAddress(i)"
                [disabled]="addresses.length === 1">
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
export class AddressSectionComponent {
  @Input() addresses: CompanyAddressForm[] = [];
  @Output() removeAddressEvent = new EventEmitter<number>();

  removeAddress(index: number) {
    this.removeAddressEvent.emit(index);
  }
}
