import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { Observable } from 'rxjs';
import { EmailType } from '../../shared/models/EmailType';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-email-type',
  imports: [],
  templateUrl: 'email-type.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EmailTypeComponent { 

  private apiUrl = `${environment.apiBaseUrl}/EmailType/get-all`;
  items = signal<EmailType[]>([])

  private http = inject(HttpClient); 

  constructor(){
    this.getAll().subscribe(data => {
      const values = (data as any).$values ?? [];
      console.info(data);
      this.items.set(data);
    })
  }

  getAll(): Observable<EmailType[]> {
    return this.http.get<EmailType[]>(this.apiUrl);
  }
}
