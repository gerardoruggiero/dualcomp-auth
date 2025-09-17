import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanyEmailForm } from '../../../models/company-register.models';

@Component({
  selector: 'app-email-section',
  imports: [CommonModule, FormsModule],
  template: `
          <div *ngFor="let email of emails; let i = index" class="contact-item">
      <div class="row">
        <div class="col-md-4">
          <div class="form-group">
            <label>Tipo de Email *</label>
            <select 
              class="form-control" 
              [(ngModel)]="email.emailType" 
              [name]="'emailType' + i"
              required>
              <option value="">Seleccione tipo</option>
              <option *ngFor="let type of email.emailTypeOptions" [value]="type.id">
                {{ type.name }}
              </option>
            </select>
          </div>
        </div>
        <div class="col-md-6">
          <div class="form-group">
            <label>Email *</label>
            <input 
              type="email" 
              class="form-control" 
              [(ngModel)]="email.email" 
              [name]="'email' + i"
              placeholder="ejemplo@empresa.com"
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
                  [(ngModel)]="email.isPrimary" 
                  [name]="'emailPrimary' + i"
                  id="emailPrimary{{ i }}">
                <label class="form-check-label" for="emailPrimary{{ i }}">
                  Principal
                </label>
              </div>
              <button 
                type="button" 
                class="btn btn-sm btn-danger mt-2"
                (click)="removeEmail(i)"
                [disabled]="emails.length === 1">
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
export class EmailSectionComponent {
  @Input() emails: CompanyEmailForm[] = [];
  @Output() removeEmailEvent = new EventEmitter<number>();

  removeEmail(index: number) {
    this.removeEmailEvent.emit(index);
  }
}
