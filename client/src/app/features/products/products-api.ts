import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Product {
  id: string;
  sku: string;
  name: string;
  price: number;
  quantity: number;
  brandId: string;
  brandName: string;
  categoryId: string;
  categoryName: string;
  vehicleIds: string[];
}

export interface GuardarProducto {
  name: string;
  price: number;
  quantity: number;
  sku: string;
  brandId: string;
  categoryId: string;
  vehicleIds: string[];
}

@Injectable({ providedIn: 'root' })
export class ProductsApi {
  private readonly http = inject(HttpClient);

  list(search: string, existencias: string): Observable<Product[]> {
    const params: Record<string, string> = {};
    const text = search.trim();
    if (text) {
      params['search'] = text;
    }
    if (existencias) {
      params['existencias'] = existencias;
    }
    return this.http.get<Product[]>(`${API_STAFF}/api/products`, { params });
  }

  create(command: GuardarProducto): Observable<Product> {
    return this.http.post<Product>(`${API_STAFF}/api/products`, command);
  }

  update(id: string, command: GuardarProducto): Observable<Product> {
    return this.http.put<Product>(`${API_STAFF}/api/products/${id}`, command);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/products/${id}`);
  }
}
