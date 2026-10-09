import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Car {
  id: string;
  brand: string;
  line: string;
  year: number;
  type: string;
  condition: string;
  mileage: number | null;
  availability: string;
  pagePrice: number | null;
  counterPrice: number | null;
  photoUrl: string;
  branchId: string;
  branch: string;
  sold: boolean;
}

export interface SaveCar {
  brand: string;
  line: string;
  year: number;
  type: string;
  condition: string;
  mileage: number | null;
  availability: string;
  pagePrice: number | null;
  counterPrice: number | null;
  photoUrl: string;
  branchId: string | null;
}

export interface Branch {
  id: string;
  code: string;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class CarsApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Car[]> {
    return this.http.get<Car[]>(`${API_STAFF}/api/cars`);
  }

  create(command: SaveCar): Observable<Car> {
    return this.http.post<Car>(`${API_STAFF}/api/cars`, command);
  }

  update(id: string, command: SaveCar): Observable<Car> {
    return this.http.put<Car>(`${API_STAFF}/api/cars/${id}`, command);
  }

  remove(id: string): Observable<void> {
    return this.http.delete<void>(`${API_STAFF}/api/cars/${id}`);
  }

  branches(): Observable<Branch[]> {
    return this.http.get<Branch[]>(`${API_STAFF}/api/branches`);
  }
}
