import { ChangeDetectionStrategy, Component, Input, Output, EventEmitter, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CompanySocialMediaForm } from '../../../models/company-register.models';

@Component({
  selector: 'app-social-media-section',
  imports: [CommonModule, FormsModule],
  template: `
          <div *ngFor="let social of socialMedias; let i = index" class="contact-item">
      <div class="row">
        <div class="col-md-4">
          <div class="form-group">
            <label>Tipo de Red Social *</label>
            <select 
              class="form-control" 
              [(ngModel)]="social.socialMediaType" 
              [name]="'socialMediaType' + i"
              required>
              <option value="">Seleccione tipo</option>
              <option *ngFor="let type of social.socialMediaTypeOptions" [value]="type.id">
                {{ type.name }}
              </option>
            </select>
          </div>
        </div>
        <div class="col-md-6">
          <div class="form-group">
            <label>URL *</label>
            <input 
              type="url" 
              class="form-control" 
              [(ngModel)]="social.url" 
              [name]="'socialUrl' + i"
              placeholder="https://www.ejemplo.com"
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
                  [(ngModel)]="social.isPrimary" 
                  [name]="'socialPrimary' + i"
                  id="socialPrimary{{ i }}">
                <label class="form-check-label" for="socialPrimary{{ i }}">
                  Principal
                </label>
              </div>
              <button 
                type="button" 
                class="btn btn-sm btn-danger mt-2"
                (click)="removeSocialMedia(i)"
                [disabled]="socialMedias.length === 1">
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
export class SocialMediaSectionComponent {
  @Input() socialMedias: CompanySocialMediaForm[] = [];
  @Output() removeSocialMediaEvent = new EventEmitter<number>();

  removeSocialMedia(index: number) {
    this.removeSocialMediaEvent.emit(index);
  }
}
