import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';
import { Sale } from '../sales/sales-api';

export interface Mesa {
  physicalStore: number;
  web: number;
  published: number;
  available: number;
  recent: Sale[];
}

@Injectable({ providedIn: 'root' })
export class HomeApi {
  private readonly http = inject(HttpClient);

  get(): Observable<Mesa> {
    return this.http.get<Mesa>(`${API_STAFF}/api/desk`);
  }
}
