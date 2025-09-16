import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-accordion-section',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './accordion-section.component.html',
  styleUrls: ['./accordion-section.component.scss']
})
export class AccordionSectionComponent {
  @Input() title: string = '';
  @Input() icon: string = '';
  @Input() count: number = 0;
  @Input() isCollapsed: boolean = false;
  @Input() addButtonText: string = 'Agregar';
  @Input() showAddButton: boolean = true;

  @Output() toggleSection = new EventEmitter<void>();
  @Output() addItem = new EventEmitter<void>();

  onToggleSection(): void {
    this.toggleSection.emit();
  }

  onAddItem(): void {
    this.addItem.emit();
  }
}
