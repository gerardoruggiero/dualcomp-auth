import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { PhoneType } from '../../shared/models/PhoneType';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-phone-type',
  imports: [],
  templateUrl: './phone-type.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PhoneTypeComponent { 
  private apiUrl = `${environment.apiBaseUrl}/PhoneType/get-all`;
  items = signal<PhoneType[]>([])

  private http = inject(HttpClient); 

  constructor(){
    this.getAll().subscribe(data => {
      const values = (data as any).$values ?? [];
      console.info(data);
      this.items.set(data);
    })
  }

  getAll(): Observable<PhoneType[]> {
    return this.http.get<PhoneType[]>(this.apiUrl);
  }
}
