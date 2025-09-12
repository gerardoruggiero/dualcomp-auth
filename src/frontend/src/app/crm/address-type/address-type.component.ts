import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { AddressType } from '../../shared/models/AddressType';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-address-type',
  imports: [],
  templateUrl: 'address-type.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AddressTypeComponent {
    private apiUrl = `${environment.apiBaseUrl}/AddressType/get-all`;
  items = signal<AddressType[]>([])

  private http = inject(HttpClient); 

  constructor(){
    this.getAll().subscribe(data => {
      const values = (data as any).$values ?? [];
      console.info(data);
      this.items.set(data);
    })
  }

  getAll(): Observable<AddressType[]> {
    return this.http.get<AddressType[]>(this.apiUrl);
  }
 }
