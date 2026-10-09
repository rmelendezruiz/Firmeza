import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface SedeResumen {
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

export interface Contacto {
  email: string;
  role: string;
  branch: string | null;
}

export interface Tablero {
  period: string;
  branch: SedeResumen;
  manager: string | null;
  sales: number;
  amount: number;
  target: number;
  progress: number;
  onSite: number;
  atCounter: number;
  inInspection: number;
  available: number;
  rows: PerformanceRow[];
  branches: SedeResumen[];
  contacts: Contacto[];
}

export interface Performance {
  period: string;
  branch: SedeResumen | null;
  row: PerformanceRow;
  contacts: Contacto[];
}

export interface Mensaje {
  id: string;
  fromEmail: string;
  toEmail: string;
  text: string;
  date: string;
}

@Injectable({ providedIn: 'root' })
export class ManagementApi {
  private readonly http = inject(HttpClient);

  board(period: string, branchId: string): Observable<Tablero> {
    let params = new HttpParams().set('period', period);
    if (branchId) {
      params = params.set('branch', branchId);
    }
    return this.http.get<Tablero>(`${API_STAFF}/api/management`, { params });
  }

  performance(period: string): Observable<Performance> {
    return this.http.get<Performance>(`${API_STAFF}/api/management/performance`, {
      params: new HttpParams().set('period', period),
    });
  }

  saveStaffCard(email: string, salary: number, commissionPercent: number): Observable<void> {
    return this.http.put<void>(`${API_STAFF}/api/management/cards`, { email, salary, commissionPercent });
  }

  saveTarget(email: string, period: string, amount: number): Observable<void> {
    return this.http.put<void>(`${API_STAFF}/api/management/targets`, { email, period, amount });
  }

  messages(con: string): Observable<Mensaje[]> {
    return this.http.get<Mensaje[]>(`${API_STAFF}/api/management/messages`, { params: new HttpParams().set('con', con) });
  }

  send(toEmail: string, text: string): Observable<Mensaje> {
    return this.http.post<Mensaje>(`${API_STAFF}/api/management/messages`, { toEmail, text });
  }
}
