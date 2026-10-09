import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { API_STAFF } from '../../core/api-url';

export interface Account {
  id: string;
  email: string;
  role: string;
  branchId: string | null;
  branch: string | null;
}

export interface CreateAccount {
  email: string;
  password: string;
  role: string;
  branchId: string | null;
}

export interface Branch {
  id: string;
  code: string;
  name: string;
}

@Injectable({ providedIn: 'root' })
export class StaffApi {
  private readonly http = inject(HttpClient);

  list(): Observable<Account[]> {
    return this.http.get<Account[]>(`${API_STAFF}/api/accounts`);
  }

  create(command: CreateAccount): Observable<Account> {
    return this.http.post<Account>(`${API_STAFF}/api/accounts`, command);
  }

  assignBranch(id: string, branchId: string): Observable<Account> {
    return this.http.put<Account>(`${API_STAFF}/api/accounts/${id}/branch`, { branchId });
  }

  branches(): Observable<Branch[]> {
    return this.http.get<Branch[]>(`${API_STAFF}/api/branches`);
  }
}
