import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Category {
  id: string;
  name: string;
}

export interface GuardarCategoria {
  name: string;
}

@Injectable({ providedIn: 'root' })
export class CategoriesApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Category[]> {
    return this.http.get<Category[]>(`${API_STAFF}/api/categories`);
  }

  create(command: GuardarCategoria): Observable<Category> {
    return this.http.post<Category>(`${API_STAFF}/api/categories`, command);
  }

  update(id: string, command: GuardarCategoria): Observable<Category> {
    return this.http.put<Category>(`${API_STAFF}/api/categories/${id}`, command);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/categories/${id}`);
  }
}
