import { ChangeDetectionStrategy, Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserService } from './services/user.service';
import { UserEntity, UserListResult, CreateUserCommand, UpdateUserCommand } from './models/user.models';
import { DataTableComponent, DataTableColumn, DataTableAction, DataTableConfig } from '../shared/components/data-table/data-table.component';
import { UserModalComponent } from './components/user-modal/user-modal.component';

@Component({
    selector: 'app-user',
    standalone: true,
    imports: [CommonModule, DataTableComponent, UserModalComponent],
    templateUrl: './user.component.html',
    changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UserComponent implements OnInit {
    private userService = inject(UserService);

    // Estado del componente
    items = signal<UserEntity[]>([]);
    totalCount = signal(0);
    currentPage = signal(1);
    totalPages = signal(1);
    pageSize = signal(10);
    searchTerm = signal('');
    isLoading = signal(false);
    isSearching = signal(false);
    errorMessage = signal('');

    // Estado del Modal
    showModal = signal(false);
    modalLoading = signal(false);
    selectedItem = signal<UserEntity | null>(null);

    // Configuración de la tabla
    columns: DataTableColumn[] = [];
    actions: DataTableAction[] = [];
    tableConfig: DataTableConfig = {
        showSearch: true,
        showPagination: true,
        pageSize: 10,
        pageSizeOptions: [5, 10, 25, 50],
        showAddButton: true,
        addButtonText: 'Nuevo Usuario',
        addButtonIcon: 'fas fa-user-plus',
        emptyMessage: 'No hay usuarios disponibles'
    };

    ngOnInit() {
        this.initializeColumns();
        this.initializeActions();
        this.loadData();
    }

    // Cargar datos
    loadData() {
        console.log('UserComponent - Cargando datos...');
        this.isLoading.set(true);
        this.errorMessage.set('');

        this.userService.getUsers(
            this.currentPage(),
            this.pageSize(),
            this.searchTerm()
        ).subscribe({
            next: (result: UserListResult) => {
                this.items.set(result.users);
                this.totalCount.set(result.totalCount);
                this.currentPage.set(result.page);
                this.totalPages.set(result.totalPages);
                this.isLoading.set(false);
                this.isSearching.set(false);
            },
            error: (error) => {
                console.error('Error loading users:', error);
                this.errorMessage.set('Error al cargar los usuarios');
                this.isLoading.set(false);
                this.isSearching.set(false);
            }
        });
    }

    // Búsqueda
    onSearch(searchTerm: string) {
        this.isSearching.set(true);
        this.searchTerm.set(searchTerm);
        this.currentPage.set(1);
        this.loadData();
    }

    // Paginación
    onPageChange(page: number) {
        if (page >= 1 && page <= this.totalPages()) {
            this.currentPage.set(page);
            this.loadData();
        }
    }

    onPageSizeChange(newPageSize: number) {
        this.pageSize.set(newPageSize);
        this.currentPage.set(1);
        this.loadData();
    }

    // Acciones
    onAdd() {
        this.selectedItem.set(null);
        this.showModal.set(true);
    }

    onEdit(item: UserEntity) {
        this.selectedItem.set(item);
        this.showModal.set(true);
    }

    onToggleStatus(item: UserEntity) {
        console.log('onToggleStatus called for:', item);
        if (!confirm(`¿Estás seguro de que deseas ${item.isActive ? 'desactivar' : 'activar'} a este usuario?`)) {
            console.log('Action cancelled by user');
            return;
        }

        console.log('Sending request to backend...');
        this.isLoading.set(true);
        const observable = item.isActive
            ? this.userService.deactivateUser(item.id)
            : this.userService.activateUser(item.id);

        observable.subscribe({
            next: () => {
                console.log('Request successful');
                this.loadData();
            },
            error: (error) => {
                console.error('Error changing user status:', error);
                this.errorMessage.set('Error al cambiar el estado del usuario');
                this.isLoading.set(false);
            }
        });
    }

    // Modal
    onModalSave(command: CreateUserCommand | UpdateUserCommand) {
        this.modalLoading.set(true);

        const observable = 'id' in command
            ? this.userService.updateUser(command.id, command)
            : this.userService.createUser(command);

        observable.subscribe({
            next: () => {
                this.modalLoading.set(false);
                this.showModal.set(false);
                this.loadData();
            },
            error: (error) => {
                console.error('Error saving user:', error);
                this.modalLoading.set(false);
                // Aquí se podría mostrar un mensaje de error específico si el backend lo devuelve
                alert('Error al guardar el usuario. Verifique los datos e intente nuevamente.');
            }
        });
    }

    onModalCancel() {
        this.showModal.set(false);
        this.selectedItem.set(null);
    }

    // Configuración
    private initializeColumns() {
        this.columns = [
            {
                key: 'fullName',
                title: 'Nombre Completo',
                sortable: false, // Backend no soporta sorting por ahora en GetUsers
                width: '25%'
            },
            {
                key: 'email',
                title: 'Email',
                sortable: false,
                width: '25%'
            },
            {
                key: 'isCompanyAdmin',
                title: 'Rol',
                sortable: false,
                width: '15%',
                render: (value: boolean) =>
                    `<span class="badge ${value ? 'badge-primary' : 'badge-info'}">${value ? 'Admin' : 'Usuario'}</span>`
            },
            {
                key: 'isActive',
                title: 'Estado',
                sortable: false,
                width: '15%',
                render: (value: boolean) =>
                    `<span class="badge ${value ? 'badge-success' : 'badge-secondary'}">${value ? 'Activo' : 'Inactivo'}</span>`
            }
        ];
    }

    private initializeActions() {
        this.actions = [
            {
                label: '',
                icon: 'fas fa-edit',
                class: 'btn-primary',
                action: (item: UserEntity) => this.onEdit(item),
                show: (item: UserEntity) => item.isActive
            },
            {
                label: '',
                icon: 'fas fa-ban',
                class: 'btn-danger',
                action: (item: UserEntity) => this.onToggleStatus(item),
                show: (item: UserEntity) => item.isActive
            },
            {
                label: '',
                icon: 'fas fa-check',
                class: 'btn-success',
                action: (item: UserEntity) => this.onToggleStatus(item),
                show: (item: UserEntity) => !item.isActive
            }
        ];
    }

    onSort(column: string) {
        // Implementar si el backend lo soporta
        console.log('Sort by:', column);
    }
}
