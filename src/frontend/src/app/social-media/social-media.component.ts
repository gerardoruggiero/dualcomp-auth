import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { SocialMediaType } from '../shared/models/SocialMediaType';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

@Component({
  selector: 'app-social-media',
  imports: [],
  templateUrl: 'social-media.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SocialMediaComponent { 
      private apiUrl = `${environment.apiBaseUrl}/SocialMediaType/get-all`;
  items = signal<SocialMediaType[]>([])

  private http = inject(HttpClient); 

  constructor(){
    this.getAll().subscribe(data => {
      const values = (data as any).$values ?? [];
      console.info(data);
      this.items.set(data);
    })
  }

  getAll(): Observable<SocialMediaType[]> {
    return this.http.get<SocialMediaType[]>(this.apiUrl);
  }
}
