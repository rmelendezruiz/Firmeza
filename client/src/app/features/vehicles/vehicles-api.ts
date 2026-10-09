import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Vehicle {
  id: string;
  brand: string;
  line: string;
  year: number;
}

export interface GuardarVehiculo {
  brand: string;
  line: string;
  year: number;
}

@Injectable({ providedIn: 'root' })
export class VehiclesApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Vehicle[]> {
    return this.http.get<Vehicle[]>(`${API_STAFF}/api/vehicles`);
  }

  create(command: GuardarVehiculo): Observable<Vehicle> {
    return this.http.post<Vehicle>(`${API_STAFF}/api/vehicles`, command);
  }

  update(id: string, command: GuardarVehiculo): Observable<Vehicle> {
    return this.http.put<Vehicle>(`${API_STAFF}/api/vehicles/${id}`, command);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/vehicles/${id}`);
  }
}
