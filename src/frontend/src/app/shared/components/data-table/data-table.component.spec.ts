import { ComponentFixture, TestBed } from '@angular/core/testing';
import { DataTableComponent, DataTableColumn, DataTableAction, DataTableConfig } from './data-table.component';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

describe('DataTableComponent', () => {
  let component: DataTableComponent;
  let fixture: ComponentFixture<DataTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DataTableComponent, CommonModule, FormsModule]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DataTableComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should display title correctly', () => {
    component.title = 'Test Table';
    component.titleIcon = 'fas fa-table';
    fixture.detectChanges();

    const titleElement = fixture.nativeElement.querySelector('.card-title');
    expect(titleElement.textContent.trim()).toContain('Test Table');
  });

  it('should display data in table rows', () => {
    const testData = [
      { id: 1, name: 'Test 1', value: 'Value 1' },
      { id: 2, name: 'Test 2', value: 'Value 2' }
    ];

    const testColumns: DataTableColumn[] = [
      { key: 'id', title: 'ID' },
      { key: 'name', title: 'Name' },
      { key: 'value', title: 'Value' }
    ];

    component.data = testData;
    component.columns = testColumns;
    fixture.detectChanges();

    const tableRows = fixture.nativeElement.querySelectorAll('tbody tr');
    expect(tableRows.length).toBe(2);
  });

  it('should display actions when provided', () => {
    const testData = [{ id: 1, name: 'Test' }];
    const testColumns: DataTableColumn[] = [{ key: 'name', title: 'Name' }];
    const testActions: DataTableAction[] = [
      {
        label: 'Edit',
        icon: 'fas fa-edit',
        class: 'btn-primary',
        action: () => {}
      }
    ];

    component.data = testData;
    component.columns = testColumns;
    component.actions = testActions;
    fixture.detectChanges();

    const actionButtons = fixture.nativeElement.querySelectorAll('.btn-group .btn');
    expect(actionButtons.length).toBe(1);
  });

  it('should emit onAdd event when add button is clicked', () => {
    spyOn(component.onAdd, 'emit');
    
    component.config = { showAddButton: true };
    fixture.detectChanges();

    const addButton = fixture.nativeElement.querySelector('.btn-primary');
    addButton.click();

    expect(component.onAdd.emit).toHaveBeenCalled();
  });

  it('should emit onSearch event when search input changes', () => {
    spyOn(component.onSearch, 'emit');
    
    component.config = { showSearch: true };
    fixture.detectChanges();

    const searchInput = fixture.nativeElement.querySelector('input[type="text"]');
    searchInput.value = 'test search';
    searchInput.dispatchEvent(new Event('input'));

    expect(component.onSearch.emit).toHaveBeenCalledWith('test search');
  });

  it('should display empty message when no data', () => {
    component.data = [];
    component.config = { emptyMessage: 'No data available' };
    fixture.detectChanges();

    const emptyMessage = fixture.nativeElement.querySelector('.text-center p');
    expect(emptyMessage.textContent.trim()).toBe('No data available');
  });

  it('should calculate page numbers correctly', () => {
    component.currentPage = 5;
    component.totalPages = 10;
    
    const pageNumbers = component.getPageNumbers();
    
    // Should show pages around current page
    expect(pageNumbers).toContain(3);
    expect(pageNumbers).toContain(4);
    expect(pageNumbers).toContain(5);
    expect(pageNumbers).toContain(6);
    expect(pageNumbers).toContain(7);
  });

  it('should render custom column values using render function', () => {
    const testData = [{ count: 5 }];
    const testColumns: DataTableColumn[] = [
      {
        key: 'count',
        title: 'Count',
        render: (value: number) => `${value} items`
      }
    ];

    component.data = testData;
    component.columns = testColumns;
    fixture.detectChanges();

    const cellContent = fixture.nativeElement.querySelector('tbody td');
    expect(cellContent.textContent.trim()).toBe('5 items');
  });
});
