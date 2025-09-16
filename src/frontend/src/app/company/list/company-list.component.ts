import { ChangeDetectionStrategy, Component, inject, signal, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { 
  DataTableComponent, 
  DataTableColumn, 
  DataTableAction, 
  DataTableConfig 
} from '../../shared/components/data-table/data-table.component';
import { GetCompanyResult, GetCompaniesResult } from '../models/company-register.models';
import { CompanyService } from '../services/CompanyService';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, DataTableComponent],
  template: `
    <div class="row">
      <div class="col-12">
        <div class="content-header">
          <div class="container-fluid">
            <div class="row mb-2">
              <div class="col-sm-6">
                <h1>Listado de Empresas</h1>
              </div>
            </div>
          </div>
        </div>

        <section class="content">
          <div class="container-fluid">
            <!-- Mensajes de estado -->
            <div *ngIf="errorMessage()" class="alert alert-danger alert-dismissible" role="alert">
              <button type="button" class="close" data-dismiss="alert" aria-hidden="true">&times;</button>
              <i class="fas fa-exclamation-triangle"></i>
              {{ errorMessage() }}
            </div>

            <!-- Loading inicial -->
            <div *ngIf="isLoading() && !isSearching()" class="text-center">
              <div class="spinner-border" role="status">
                <span class="sr-only">Cargando...</span>
              </div>
              <p class="mt-2">Cargando empresas...</p>
            </div>

            <!-- Data Table -->
            <app-data-table
              *ngIf="!isLoading() || isSearching()"
              title="Empresas"
              titleIcon="fas fa-building"
              [columns]="columns"
              [data]="companies()"
              [actions]="actions"
              [config]="tableConfig"
              [currentPage]="currentPage()"
              [totalPages]="totalPages()"
              [totalCount]="totalCount()"
              [pageSize]="pageSize()"
              [searchValue]="searchTerm()"
              (onAdd)="navigateToRegister()"
              (onSearch)="onSearch($event)"
              (onSort)="onSort($event)"
              (onPageChange)="onPageChange($event)"
              (onPageSizeChange)="onPageSizeChange($event)">
            </app-data-table>
          </div>
        </section>
      </div>
    </div>
  `,
  styleUrls: ['./company-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CompanyListComponent implements OnInit {
  router = inject(Router);
  private companyService = inject(CompanyService);

  // Estado del componente
  isLoading = signal(false);
  errorMessage = signal('');
  searchTerm = signal('');
  isSearching = signal(false);

  // Datos de la tabla
  companies = signal<GetCompanyResult[]>([]);
  currentPage = signal(1);
  totalPages = signal(1);
  totalCount = signal(0);
  pageSize = signal(10);

  // Configuración de la tabla
  columns: DataTableColumn[] = [
    {
      key: 'name',
      title: 'Nombre',
      sortable: true,
      width: '60%'
    },
    {
      key: 'taxId',
      title: 'Tax ID',
      sortable: true,
      width: '40%'
    }
  ];

  actions: DataTableAction[] = [];

  tableConfig: DataTableConfig = {
    showSearch: true,
    showPagination: true,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50],
    showAddButton: true,
    addButtonText: 'Nueva Empresa',
    addButtonIcon: 'fas fa-plus',
    emptyMessage: 'No hay empresas registradas'
  };

  ngOnInit() {
    this.loadCompanies();
    this.initializeActions();
  }

  private initializeActions() {
    this.actions = [
      {
        label: '',
        icon: 'fas fa-edit',
        class: 'btn-primary',
        action: this.editCompany.bind(this)
      }
    ];
  }

  private loadCompanies() {
    this.isLoading.set(true);
    this.errorMessage.set('');

    this.companyService.getCompanies(
      this.currentPage(),
      this.pageSize(),
      this.searchTerm()
    ).subscribe({
      next: (result: GetCompaniesResult) => {
        this.companies.set(result.companies);
        this.currentPage.set(result.page);
        this.totalPages.set(result.totalPages);
        this.totalCount.set(result.totalCount);
        this.isLoading.set(false);
        this.isSearching.set(false);
      },
      error: (error) => {
        console.error('Error loading companies:', error);
        this.errorMessage.set('Error al cargar las empresas');
        this.isLoading.set(false);
        this.isSearching.set(false);
      }
    });
  }

  onSearch(searchTerm: string) {
    this.isSearching.set(true);
    this.searchTerm.set(searchTerm);
    this.currentPage.set(1); // Reset to first page when searching
    this.loadCompanies();
  }

  onSort(column: string) {
    // TODO: Implement sorting logic
    console.log('Sort by:', column);
  }

  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadCompanies();
    }
  }

  onPageSizeChange(newPageSize: number) {
    this.pageSize.set(newPageSize);
    this.currentPage.set(1); // Reset to first page when changing page size
    this.loadCompanies();
  }

  navigateToRegister() {
    this.router.navigate(['/company/register']);
  }

  editCompany(company: GetCompanyResult) {
    console.log('Edit company clicked:', company);
    console.log('Navigating to:', `/company/edit/${company.id}`);
    this.router.navigate(['/company/edit', company.id]);
  }

  viewCompany(company: GetCompanyResult) {
    // TODO: Implement view functionality
    console.log('View company:', company);
    // For now, navigate to edit
    this.router.navigate(['/company/edit', company.id]);
  }
}
