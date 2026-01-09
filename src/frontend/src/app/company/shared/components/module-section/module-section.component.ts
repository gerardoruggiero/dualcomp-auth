import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BaseTypeClass } from '../../../../shared/models/BaseType';

@Component({
  selector: 'app-module-section',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="row">
      <div class="col-12">
        <label class="form-label d-block">Módulos Asignados <span class="text-danger">*</span></label>
        <div class="module-grid mt-2">
          <div *ngFor="let module of moduleOptions" class="module-item mb-2">
            <div class="form-check form-switch">
              <input 
                class="form-check-input" 
                type="checkbox" 
                [id]="'module_' + module.id"
                [checked]="isModuleSelected(module.id)"
                (change)="toggleModule(module.id)">
              <label class="form-check-label" [for]="'module_' + module.id">
                {{ module.name }}
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .module-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 15px;
      background: #f8f9fa;
      padding: 15px;
      border-radius: 8px;
      border: 1px solid #dee2e6;
    }
    .module-item {
      padding: 5px;
    }
    .form-check-input:checked {
        background-color: var(--primary-color, #0d6efd);
        border-color: var(--primary-color, #0d6efd);
    }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ModuleSectionComponent {
  @Input() moduleOptions: BaseTypeClass[] = [];
  @Input() selectedModuleIds: string[] = [];
  @Output() selectedModuleIdsChange = new EventEmitter<string[]>();

  isModuleSelected(moduleId: string): boolean {
    return this.selectedModuleIds.includes(moduleId);
  }

  toggleModule(moduleId: string): void {
    const updatedIds = [...this.selectedModuleIds];
    const index = updatedIds.indexOf(moduleId);

    if (index === -1) {
      updatedIds.push(moduleId);
    } else {
      updatedIds.splice(index, 1);
    }

    this.selectedModuleIdsChange.emit(updatedIds);
  }
}
