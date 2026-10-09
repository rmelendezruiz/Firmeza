import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface SaleLine {
  productId: string;
  productName: string;
  quantity: number;
  price: number;
}

export interface Sale {
  id: string;
  number: string;
  date: string;
  customerId: string;
  customerName: string;
  branchId: string;
  branch: string;
  channel: string;
  paymentMethod: string;
  status: 'cerrada' | 'preliminar' | 'anulada' | string;
  carId: string | null;
  carName: string | null;
  total: number;
  iva: number;
  inspectionDeposit: number | null;
  inspectionDeadline: string | null;
  sellerEmail: string;
  lines: SaleLine[];
}

export interface RegisterSale {
  customerId: string;
  channel: string;
  paymentMethod: string;
  carId: string | null;
  lines: { productId: string; quantity: number }[];
}

@Injectable({ providedIn: 'root' })
export class SalesApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Sale[]> {
    return this.http.get<Sale[]>(`${API_STAFF}/api/sales`);
  }

  register(command: RegisterSale): Observable<Sale> {
    return this.http.post<Sale>(`${API_STAFF}/api/sales`, command);
  }

  closeInspection(id: string): Observable<void> {
    return this.http.post<void>(`${API_STAFF}/api/sales/${id}/close`, {});
  }

  cancel(id: string): Observable<void> {
    return this.http.post<void>(`${API_STAFF}/api/sales/${id}/cancel`, {});
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/sales/${id}`);
  }
}
