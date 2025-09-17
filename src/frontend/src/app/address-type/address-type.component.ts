import { ChangeDetectionStrategy, Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TypeModalComponent, TypeModalConfig } from '../shared/components/type-modal/type-modal.component';
import { AddressTypeService } from '../shared/services/address-type.service';
import { 
  AddressTypeEntity, 
  CreateAddressTypeCommand, 
  UpdateAddressTypeCommand,
  AddressTypeListResult 
} from '../shared/models/address-type.models';
import { 
  DataTableComponent,
  DataTableColumn, 
  DataTableAction,
  DataTableConfig 
} from '../shared/components/data-table/data-table.component';

@Component({
  selector: 'app-address-type',
  standalone: true,
  imports: [CommonModule, DataTableComponent, TypeModalComponent],
  templateUrl: './address-type.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddressTypeComponent implements OnInit {
  private addressTypeService = inject(AddressTypeService);

  // Estado del componente
  items = signal<AddressTypeEntity[]>([]);
  totalCount = signal(0);
  currentPage = signal(1);
  totalPages = signal(1);
  pageSize = signal(10);
  searchTerm = signal('');
  isLoading = signal(false);
  isSearching = signal(false);
  errorMessage = signal('');
  showModal = signal(false);
  modalLoading = signal(false);
  selectedItem = signal<AddressTypeEntity | null>(null);

  // Configuración de la tabla
  columns: DataTableColumn[] = [];
  actions: DataTableAction[] = [];
  tableConfig: DataTableConfig = {
    showSearch: true,
    showPagination: true,
    pageSize: 10,
    pageSizeOptions: [5, 10, 25, 50],
    showAddButton: true,
    addButtonText: 'Agregar',
    addButtonIcon: 'fas fa-plus',
    emptyMessage: 'No hay tipos de dirección disponibles'
  };

  // Configuración del modal
  modalConfig: TypeModalConfig = {
    title: 'Tipo de Dirección',
    nameLabel: 'Nombre del Tipo',
    descriptionLabel: 'Descripción',
    namePlaceholder: 'Ej: Casa, Oficina, Almacén',
    descriptionPlaceholder: 'Descripción del tipo de dirección',
    saveButtonText: 'Guardar',
    cancelButtonText: 'Cancelar',
    saveButtonClass: 'btn-primary'
  };

  ngOnInit() {
    this.initializeColumns();
    this.initializeActions();
    // Siempre cargar datos del servidor al inicializar
    this.loadDataFromServer();
  }

  // Cargar datos del servidor (siempre)
  loadDataFromServer() {
    console.log('AddressTypeComponent - Cargando datos del servidor...');
    this.isLoading.set(true);
    this.errorMessage.set('');
    
    this.addressTypeService.getTypes(
      this.currentPage(),
      this.pageSize(),
      this.searchTerm()
    ).subscribe({
      next: (result: AddressTypeListResult) => {
        console.log('AddressTypeComponent - Datos recibidos del servidor:', result);
        this.items.set(result.items);
        this.totalCount.set(result.totalCount);
        this.currentPage.set(result.page);
        this.totalPages.set(result.totalPages);
        this.isLoading.set(false);
        this.isSearching.set(false);
      },
      error: (error) => {
        console.error('Error loading address types:', error);
        this.errorMessage.set('Error al cargar los tipos de dirección');
        this.isLoading.set(false);
        this.isSearching.set(false);
      }
    });
  }

  // Cargar datos (método interno)
  loadData() {
    this.loadDataFromServer();
  }

  // Manejar búsqueda
  onSearch(searchTerm: string) {
    this.isSearching.set(true);
    this.searchTerm.set(searchTerm);
    this.currentPage.set(1); // Reset to first page when searching
    this.loadData();
  }

  // Manejar ordenamiento
  onSort(column: string) {
    // TODO: Implement sorting logic
    console.log('Sort by:', column);
  }

  // Manejar cambio de página
  onPageChange(page: number) {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadData();
    }
  }

  // Manejar cambio de tamaño de página
  onPageSizeChange(newPageSize: number) {
    this.pageSize.set(newPageSize);
    this.currentPage.set(1); // Reset to first page when changing page size
    this.loadData();
  }

  onAddType() {
    this.selectedItem.set(null);
    this.showModal.set(true);
  }

  onEditType(item: AddressTypeEntity) {
    console.log('AddressTypeComponent - Editando tipo:', item);
    // Cargar datos frescos del servidor antes de abrir el modal
    this.addressTypeService.getTypeById(item.id).subscribe({
      next: (freshItem: AddressTypeEntity) => {
        console.log('AddressTypeComponent - Datos frescos del servidor:', freshItem);
        this.selectedItem.set(freshItem);
        this.showModal.set(true);
      },
      error: (error) => {
        console.error('Error loading address type for edit:', error);
        // Fallback: usar los datos que tenemos
        this.selectedItem.set(item);
        this.showModal.set(true);
      }
    });
  }


  // Manejar eventos del modal
  onModalSave(command: CreateAddressTypeCommand | UpdateAddressTypeCommand) {
    this.modalLoading.set(true);

    const observable = 'id' in command
      ? this.addressTypeService.updateType(command.id, command)
      : this.addressTypeService.createType(command);

    observable.subscribe({
      next: () => {
        console.log('AddressTypeComponent - Guardado exitoso, recargando datos del servidor...');
        this.modalLoading.set(false);
        this.showModal.set(false);
        // Siempre recargar desde el servidor después de guardar
        this.loadDataFromServer();
      },
      error: (error) => {
        console.error('Error saving address type:', error);
        this.modalLoading.set(false);
      }
    });
  }

  onModalCancel() {
    this.showModal.set(false);
    this.selectedItem.set(null);
  }

  // Configurar columnas de la tabla
  private initializeColumns() {
    this.columns = [
      {
        key: 'name',
        title: 'Nombre',
        sortable: true,
        width: '30%'
      },
      {
        key: 'description',
        title: 'Descripción',
        sortable: false,
        width: '40%'
      },
      {
        key: 'isActive',
        title: 'Estado',
        sortable: true,
        width: '15%',
        render: (value: boolean) => 
          `<span class="badge ${value ? 'badge-success' : 'badge-secondary'}">${value ? 'Activo' : 'Inactivo'}</span>`
      }
    ];
  }

  // Configurar acciones de la tabla
  private initializeActions() {
    this.actions = [
      {
        label: '',
        icon: 'fas fa-edit',
        class: 'btn-primary',
        action: (item: AddressTypeEntity) => this.onEditType(item),
        show: (item: AddressTypeEntity) => item.isActive
      }
    ];
  }
}
