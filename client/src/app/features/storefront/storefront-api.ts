import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_SITE } from '../../core/api-url';
import { formatMoney } from '../../shared/format';

export interface PublishedCar {
  id: string;
  brand: string;
  line: string;
  year: number;
  type: string;
  condition: string;
  mileage: number | null;
  availability: 'pagina' | 'counter' | 'ambos' | string;
  pagePrice: number | null;
  counterPrice: number | null;
  photoUrl: string;
  branchCode: string;
  branch: string;
  city: string;
}

export interface ShowcaseFilter {
  brand?: string;
  line?: string;
  city?: string;
  yearFrom?: string;
  yearTo?: string;
  priceFrom?: string;
  priceTo?: string;
}

export interface CompraHecha {
  number: string;
  car: string;
  total: number;
  inspectionDeposit: number | null;
  inspectionDeadline: string | null;
  status: string;
  paymentMethod: string;
  counter: string;
  inspectionPhone: string;
}

export interface BranchPhoto {
  url: string;
  description: string;
}

export interface PublicBranch {
  id: string;
  code: string;
  name: string;
  city: string;
  headquarters: string;
  phone: string;
  inspectionPhone: string;
  email: string;
  hours: string;
  latitude: number;
  longitude: number;
  available: number;
  photos: BranchPhoto[];
}

export interface CatalogPart {
  id: string;
  sku: string;
  name: string;
  price: number;
  brand: string;
  category: string;
  available: boolean;
  fitments: string[];
}

export interface OpenAccount {
  name: string;
  email: string;
  document: string;
  phone: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class StorefrontApi {
  private readonly http = inject(HttpClient);

  showcase(filter: ShowcaseFilter = {}): Observable<PublishedCar[]> {
    let params = new HttpParams();
    for (const [key, value] of Object.entries(filter)) {
      if (value?.trim()) {
        params = params.set(key, value.trim());
      }
    }
    return this.http.get<PublishedCar[]>(`${API_SITE}/api/showcase`, { params });
  }

  cars(): Observable<PublishedCar[]> {
    return this.http.get<PublishedCar[]>(`${API_SITE}/api/catalog/cars`);
  }

  parts(search: string): Observable<CatalogPart[]> {
    let params = new HttpParams();
    const text = search.trim();
    if (text) {
      params = params.set('search', text);
    }
    return this.http.get<CatalogPart[]>(`${API_SITE}/api/catalog/parts`, { params });
  }

  open(command: OpenAccount): Observable<void> {
    return this.http.post<void>(`${API_SITE}/api/account`, command);
  }

  buy(carId: string, acceptsTerms: boolean, seenPrice: number, paymentMethod: string): Observable<CompraHecha> {
    return this.http.post<CompraHecha>(`${API_SITE}/api/catalog/purchases`, {
      carId,
      acceptsTerms,
      seenPrice,
      paymentMethod,
    });
  }

  branches(): Observable<PublicBranch[]> {
    return this.http.get<PublicBranch[]>(`${API_SITE}/api/branches`);
  }

  branch(code: string): Observable<PublicBranch> {
    return this.http.get<PublicBranch>(`${API_SITE}/api/branches/${code}`);
  }
}

export function priceLines(car: PublishedCar): string[] {
  const web = car.pagePrice === null ? '' : formatMoney(car.pagePrice);
  const counter = car.counterPrice === null ? '' : formatMoney(car.counterPrice);
  if (car.availability === 'ambos') {
    return [`Precio página: ${web}`, `Precio counter: ${counter}`];
  }
  if (car.availability === 'pagina') {
    return [`Precio página — vehículo en bodega: ${web}`];
  }
  return [`${counter || web}. Solo en la sucursal.`];
}
