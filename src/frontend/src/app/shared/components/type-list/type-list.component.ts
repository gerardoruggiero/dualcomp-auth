import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { DataTableComponent, DataTableColumn, DataTableAction, DataTableConfig } from '../data-table/data-table.component';
import { ContentHeaderComponent, ContentHeaderAction } from '../content-header/content-header.component';
import { BaseTypeEntity, BaseTypeListResult } from '../../models/base-type.models';

export interface TypeListConfig {
  title: string;
  titleIcon: string;
  addButtonText: string;
  addButtonIcon: string;
  emptyMessage: string;
  columns: DataTableColumn[];
  actions: DataTableAction[];
}

@Component({
  selector: 'app-type-list',
  standalone: true,
  imports: [CommonModule, FormsModule, DataTableComponent, ContentHeaderComponent],
  template: `
    <div class="content-wrapper">
      <!-- Header -->
        <app-content-header 
        [title]="config.title"
        [actions]="headerActions">
      </app-content-header>

      <!-- Tabla de datos -->
      <div class="content">
        <div class="container-fluid">
          <app-data-table
            [title]="config.title"
            [columns]="config.columns"
            [data]="items()"
            [actions]="config.actions"
            [config]="tableConfig"
            [currentPage]="currentPage()"
            [totalPages]="totalPages()"
            [totalCount]="totalCount()"
            [pageSize]="pageSize()"
            [searchValue]="searchTerm()"
            (onAdd)="onAdd()"
            (onSearch)="onSearch($event)"
            (onSort)="onSort($event)"
            (onPageChange)="onPageChange($event)"
            (onPageSizeChange)="onPageSizeChange($event)">
          </app-data-table>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./type-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TypeListComponent implements OnInit, OnDestroy {
  @Input() config!: TypeListConfig;
  @Input() items = signal<BaseTypeEntity[]>([]);
  @Input() totalCount = signal(0);
  @Input() currentPage = signal(1);
  @Input() totalPages = signal(1);
  @Input() pageSize = signal(10);
  @Input() searchTerm = signal('');
  @Input() isLoading = signal(false);

  @Output() onLoadData = new EventEmitter<{ page: number; pageSize: number; searchTerm: string }>();
  @Output() onAddType = new EventEmitter<void>();
  @Output() onEditType = new EventEmitter<BaseTypeEntity>();
  @Output() onDeleteType = new EventEmitter<BaseTypeEntity>();

  // Configuración de la tabla
  tableConfig: DataTableConfig = {
    showSearch: true,
    showPagination: true,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50],
    showAddButton: true,
    addButtonText: 'Agregar',
    addButtonIcon: 'fas fa-plus',
    emptyMessage: 'No hay tipos disponibles'
  };

  // Acciones del header
  headerActions: ContentHeaderAction[] = [];

  private destroy$ = new Subject<void>();
  private searchSubject = new Subject<string>();

  ngOnInit() {
    // Configurar debounce para búsqueda
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(searchTerm => {
        this.searchTerm.set(searchTerm);
        this.currentPage.set(1); // Reset a la primera página
        this.loadData();
      });

    // Cargar datos iniciales
    this.loadData();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // Cargar datos
  private loadData() {
    this.onLoadData.emit({
      page: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.searchTerm()
    });
  }

  // Manejar agregar
  onAdd() {
    this.onAddType.emit();
  }

  // Manejar búsqueda
  onSearch(searchTerm: string) {
    this.searchSubject.next(searchTerm);
  }

  // Manejar ordenamiento
  onSort(column: string) {
    // Implementar lógica de ordenamiento si es necesario
    console.log('Sort by:', column);
  }

  // Manejar cambio de página
  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadData();
  }

  // Manejar cambio de tamaño de página
  onPageSizeChange(newPageSize: number) {
    this.pageSize.set(newPageSize);
    this.currentPage.set(1); // Reset a la primera página
    this.loadData();
  }

  // Métodos auxiliares para las acciones de la tabla
  editItem(item: BaseTypeEntity) {
    this.onEditType.emit(item);
  }

  deleteItem(item: BaseTypeEntity) {
    if (confirm(`¿Está seguro de que desea desactivar "${item.name}"?`)) {
      this.onDeleteType.emit(item);
    }
  }

  // Verificar si se puede editar
  canEdit(item: BaseTypeEntity): boolean {
    return item.isActive;
  }

  // Verificar si se puede eliminar
  canDelete(item: BaseTypeEntity): boolean {
    return item.isActive;
  }

  // Obtener clase CSS para el estado
  getStatusClass(item: BaseTypeEntity): string {
    return item.isActive ? 'text-success' : 'text-muted';
  }

  // Obtener texto del estado
  getStatusText(item: BaseTypeEntity): string {
    return item.isActive ? 'Activo' : 'Inactivo';
  }
}
