import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Brand {
  id: string;
  name: string;
}

export interface GuardarMarca {
  name: string;
}

@Injectable({ providedIn: 'root' })
export class BrandsApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Brand[]> {
    return this.http.get<Brand[]>(`${API_STAFF}/api/brands`);
  }

  create(command: GuardarMarca): Observable<Brand> {
    return this.http.post<Brand>(`${API_STAFF}/api/brands`, command);
  }

  update(id: string, command: GuardarMarca): Observable<Brand> {
    return this.http.put<Brand>(`${API_STAFF}/api/brands/${id}`, command);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/brands/${id}`);
  }
}
