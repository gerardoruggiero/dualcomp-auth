import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import {
    UserEntity,
    CreateUserCommand,
    UpdateUserCommand,
    UserListResult
} from '../models/user.models';

@Injectable({
    providedIn: 'root'
})
export class UserService {
    private http = inject(HttpClient);
    private apiUrl = `${environment.apiBaseUrl}/UserManagement`;

    getUsers(page = 1, pageSize = 10, searchTerm = ''): Observable<UserListResult> {
        let params = new HttpParams()
            .set('page', page.toString())
            .set('pageSize', pageSize.toString());

        if (searchTerm) {
            params = params.set('searchTerm', searchTerm);
        }

        return this.http.get<any>(this.apiUrl, { params }).pipe(
            map(response => {
                // Mapear la respuesta para asegurar que fullName esté disponible
                if (response && response.users) {
                    response.users = response.users.map((user: UserEntity) => ({
                        ...user,
                        fullName: `${user.firstName} ${user.lastName}`.trim()
                    }));
                }
                return response;
            })
        );
    }

    createUser(command: CreateUserCommand): Observable<any> {
        return this.http.post(this.apiUrl, command);
    }

    updateUser(id: string, command: UpdateUserCommand): Observable<any> {
        return this.http.put(`${this.apiUrl}/${id}`, command);
    }

    deactivateUser(id: string): Observable<any> {
        return this.http.delete(`${this.apiUrl}/${id}`);
    }

    activateUser(id: string): Observable<any> {
        return this.http.patch(`${this.apiUrl}/${id}/activate`, {});
    }
}
