import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface ContentHeaderAction {
  label: string;
  icon?: string;
  class?: string;
  disabled?: boolean;
  onClick: () => void;
}

@Component({
  selector: 'app-content-header',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="content-header">
      <div class="container-fluid">
        <div class="row mb-2">
          <div class="col-sm-6">
            <h1>{{ title }}</h1>
          </div>
          <div class="col-sm-6" *ngIf="actions && actions.length > 0">
            <div class="float-sm-right d-flex align-items-center gap-2">
              <button 
                *ngFor="let action of actions"
                type="button" 
                [class]="action.class || 'btn btn-secondary'"
                [disabled]="action.disabled"
                (click)="action.onClick()">
                <i *ngIf="action.icon" [class]="action.icon"></i>
                {{ action.label }}
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./content-header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ContentHeaderComponent {
  @Input() title: string = '';
  @Input() actions: ContentHeaderAction[] = [];
}
