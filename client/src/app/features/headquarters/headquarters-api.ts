import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface BranchSummary {
  id: string;
  code: string;
  name: string;
  city: string;
}

export interface PerformanceRow {
  email: string;
  branch: string | null;
  sales: number;
  amount: number;
  target: number;
  progress: number;
  salary: number;
  commissionPercent: number;
  commission: number;
}

export interface Asset {
  name: string;
  type: 'equipo' | 'infraestructura' | string;
  quantity: number;
  status: 'operativo' | 'mantenimiento' | string;
}

export interface HeadquartersCounter {
  branch: BranchSummary;
  headquarters: string;
  phone: string;
  hours: string;
  capacity: number;
  areaM2: number;
  manager: string | null;
  salespeople: number;
  sales: number;
  amount: number;
  target: number;
  progress: number;
  onSite: number;
  atCounter: number;
  inInspection: number;
  topSeller: PerformanceRow | null;
  onFloor: number;
  inWarehouse: number;
  inventoryValue: number;
  assets: Asset[];
}

export interface PaymentMix {
  channel: 'pagina' | 'punto' | string;
  method: string;
  sales: number;
  amount: number;
}

export interface TopCar {
  city: string;
  car: string | null;
  units: number;
  amount: number;
}

export interface Inventory {
  cars: number;
  onFloor: number;
  inWarehouse: number;
  value: number;
  parts: number;
  units: number;
  lowStock: { sku: string; name: string; quantity: number }[];
}

export interface Headquarters {
  period: string;
  sales: number;
  amount: number;
  inInspection: number;
  counters: HeadquartersCounter[];
  payments: PaymentMix[];
  top: TopCar[];
  ranking: PerformanceRow[];
  inventory: Inventory;
}

@Injectable({ providedIn: 'root' })
export class HeadquartersApi {
  private readonly http = inject(HttpClient);

  get(period: string): Observable<Headquarters> {
    return this.http.get<Headquarters>(`${API_STAFF}/api/management/headquarters`, {
      params: new HttpParams().set('period', period),
    });
  }
}
