import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Customer {
  id: string;
  name: string;
  email: string;
  document: string;
  phone: string;
}

export interface SaveCustomer {
  name: string;
  email: string;
  document: string;
  phone: string;
}

@Injectable({ providedIn: 'root' })
export class CustomersApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Customer[]> {
    return this.http.get<Customer[]>(`${API_STAFF}/api/customers`);
  }

  create(command: SaveCustomer): Observable<Customer> {
    return this.http.post<Customer>(`${API_STAFF}/api/customers`, command);
  }

  update(id: string, command: SaveCustomer): Observable<Customer> {
    return this.http.put<Customer>(`${API_STAFF}/api/customers/${id}`, command);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/customers/${id}`);
  }
}
