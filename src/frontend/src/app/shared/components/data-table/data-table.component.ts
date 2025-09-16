import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy, OnInit, OnDestroy, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

export interface DataTableColumn {
  key: string;
  title: string;
  sortable?: boolean;
  render?: (value: any, row: any) => string;
  width?: string;
}

export interface DataTableAction {
  label: string;
  icon: string;
  class: string;
  action: (row: any) => void;
  show?: (row: any) => boolean;
}

export interface DataTableConfig {
  showSearch?: boolean;
  showPagination?: boolean;
  pageSize?: number;
  pageSizeOptions?: number[];
  showAddButton?: boolean;
  addButtonText?: string;
  addButtonIcon?: string;
  emptyMessage?: string;
}

@Component({
  selector: 'app-data-table',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="card">
      <div class="card-header">
        <h3 class="card-title">
          <i *ngIf="titleIcon" [class]="titleIcon"></i>
          {{ title }}
        </h3>
        <div class="card-tools">
          <button 
            *ngIf="config.showAddButton !== false" 
            class="btn btn-primary" 
            (click)="onAdd.emit()">
            <i [class]="config.addButtonIcon || 'fas fa-plus'"></i> 
            {{ config.addButtonText || 'Agregar' }}
          </button>
        </div>
      </div>
      
      <div class="card-body">
        <!-- Barra de búsqueda -->
        <div *ngIf="config.showSearch !== false" class="row mb-3">
          <div class="col-md-6">
            <div class="input-group">
              <div class="input-group-prepend">
                <span class="input-group-text">
                  <i *ngIf="!isSearching" class="fas fa-search"></i>
                  <i *ngIf="isSearching" class="fas fa-spinner fa-spin"></i>
                </span>
              </div>
              <input 
                type="text" 
                class="form-control" 
                placeholder="Buscar..."
                [(ngModel)]="searchTerm"
                (input)="onSearchInput(searchTerm)">
              <div class="input-group-append" *ngIf="searchTerm">
                <button 
                  class="btn btn-outline-secondary" 
                  type="button"
                  (click)="clearSearch()"
                  title="Limpiar búsqueda">
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Tabla -->
        <div class="table-responsive" [class.transitioning]="isTransitioning">
          <table class="table table-bordered table-striped table-hover">
            <thead class="thead-light">
              <tr>
                <th 
                  *ngFor="let col of columns" 
                  [style.width]="col.width"
                  [class.sortable]="col.sortable"
                  (click)="col.sortable ? onSort.emit(col.key) : null">
                  {{ col.title }}
                  <i *ngIf="col.sortable" class="fas fa-sort float-right"></i>
                </th>
                <th *ngIf="actions.length > 0" style="width: 120px;">Acciones</th>
              </tr>
            </thead>
            <tbody>
              <tr *ngFor="let row of data; let i = index" [class.table-active]="selectedRow === i">
                <td *ngFor="let col of columns">
                  <span [innerHTML]="col.render ? col.render(row[col.key], row) : row[col.key]"></span>
                </td>
                <td *ngIf="actions.length > 0">
                  <div class="btn-group" role="group">
                    <button 
                      *ngFor="let action of actions" 
                      type="button"
                      class="btn btn-sm" 
                      [class]="action.class"
                      (click)="action.action(row)"
                      [style.display]="action.show && !action.show(row) ? 'none' : 'inline-block'"
                      [title]="action.label">
                      <i [class]="action.icon"></i>
                      <span class="d-none d-md-inline">{{ action.label }}</span>
                    </button>
                  </div>
                </td>
              </tr>
            </tbody>
          </table>
        </div>

        <!-- Mensaje cuando no hay datos -->
        <div *ngIf="data.length === 0" class="text-center py-4">
          <i class="fas fa-inbox fa-3x text-muted mb-3"></i>
          <p class="text-muted">{{ config.emptyMessage || 'No hay datos disponibles' }}</p>
        </div>

        <!-- Paginación -->
        <div *ngIf="config.showPagination !== false && totalPages > 1" class="row mt-3">
          <div class="col-md-6">
            <div class="dataTables_info">
              Mostrando {{ (currentPage - 1) * pageSize + 1 }} a 
              {{ Math.min(currentPage * pageSize, totalCount) }} de 
              {{ totalCount }} registros
            </div>
          </div>
          <div class="col-md-6">
            <div class="dataTables_paginate paging_simple_numbers">
              <ul class="pagination pagination-sm float-right">
                <li class="paginate_button page-item previous" [class.disabled]="currentPage === 1">
                  <a class="page-link" (click)="onPageChange.emit(currentPage - 1)">Anterior</a>
                </li>
                
                <li 
                  *ngFor="let page of getPageNumbers()" 
                  class="paginate_button page-item" 
                  [class.active]="page === currentPage">
                  <a class="page-link" (click)="onPageChange.emit(page)">{{ page }}</a>
                </li>
                
                <li class="paginate_button page-item next" [class.disabled]="currentPage === totalPages">
                  <a class="page-link" (click)="onPageChange.emit(currentPage + 1)">Siguiente</a>
                </li>
              </ul>
            </div>
          </div>
        </div>

        <!-- Selector de tamaño de página -->
        <div *ngIf="config.showPagination !== false && config.pageSizeOptions" class="row mt-2">
          <div class="col-md-6">
            <div class="dataTables_length">
              <label>
                Mostrar 
                <select 
                  class="form-control form-control-sm d-inline-block w-auto" 
                  [(ngModel)]="pageSize"
                  (change)="onPageSizeChange.emit(pageSize)">
                  <option *ngFor="let size of config.pageSizeOptions" [value]="size">
                    {{ size }}
                  </option>
                </select> 
                registros
              </label>
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styleUrls: ['./data-table.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class DataTableComponent implements OnInit, OnDestroy, OnChanges {
  @Input() title: string = '';
  @Input() titleIcon?: string;
  @Input() columns: DataTableColumn[] = [];
  @Input() data: any[] = [];
  @Input() actions: DataTableAction[] = [];
  @Input() config: DataTableConfig = {};
  @Input() currentPage: number = 1;
  @Input() totalPages: number = 1;
  @Input() totalCount: number = 0;
  @Input() pageSize: number = 10;
  @Input() selectedRow?: number;
  @Input() searchValue: string = '';

  @Output() onAdd = new EventEmitter<void>();
  @Output() onSearch = new EventEmitter<string>();
  @Output() onSort = new EventEmitter<string>();
  @Output() onPageChange = new EventEmitter<number>();
  @Output() onPageSizeChange = new EventEmitter<number>();

  searchTerm: string = '';
  Math = Math; // Para usar Math en el template
  isSearching = false;
  isTransitioning = false;
  
  private searchSubject = new Subject<string>();

  constructor(private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    // Configurar debounce para la búsqueda
    this.searchSubject
      .pipe(
        debounceTime(300), // Esperar 300ms después del último input
        distinctUntilChanged() // Solo emitir si el valor cambió
      )
      .subscribe(searchTerm => {
        this.isSearching = false;
        this.isTransitioning = true;
        this.onSearch.emit(searchTerm);
        
        // Pequeño delay para suavizar la transición
        setTimeout(() => {
          this.isTransitioning = false;
          this.cdr.markForCheck();
        }, 150);
      });
  }

  ngOnChanges(changes: SimpleChanges) {
    // Sincronizar el searchTerm interno con el valor que viene del padre
    if (changes['searchValue'] && changes['searchValue'].currentValue !== undefined) {
      this.searchTerm = changes['searchValue'].currentValue;
    }
  }

  ngOnDestroy() {
    this.searchSubject.complete();
  }

  onSearchInput(searchTerm: string) {
    this.isSearching = true;
    this.searchSubject.next(searchTerm);
  }

  clearSearch() {
    this.searchTerm = '';
    this.onSearch.emit('');
  }

  getPageNumbers(): number[] {
    const pages: number[] = [];
    const maxVisible = 5;
    let start = Math.max(1, this.currentPage - Math.floor(maxVisible / 2));
    let end = Math.min(this.totalPages, start + maxVisible - 1);
    
    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }
    
    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    
    return pages;
  }
}
