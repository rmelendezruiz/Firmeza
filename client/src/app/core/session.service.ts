import { HttpClient } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, map, Observable, of, tap } from 'rxjs';
import { API_STAFF, API_SITE } from './api-url';

const KEY = 'firmeza.session';

export interface Session {
  token: string;
  email: string;
  role: string;
}

export interface StaffSessionProfile {
  email: string;
  role: string;
  branchId: string | null;
  branch: string | null;
}

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly http = inject(HttpClient);
  private readonly session = signal<Session | null>(read());
  private readonly profile = signal<StaffSessionProfile | null>(null);

  readonly isActive = computed(() => this.session() !== null);
  readonly email = computed(() => this.session()?.email ?? '');
  readonly role = computed(() => this.session()?.role ?? '');
  readonly branchId = computed(() => this.profile()?.branchId ?? null);
  readonly branch = computed(() => this.profile()?.branch ?? null);

  token(): string | null {
    return this.session()?.token ?? null;
  }

  signIn(email: string, password: string): Observable<Session> {
    return this.http
      .post<Session>(`${API_STAFF}/api/session`, { email, password })
      .pipe(tap((session) => this.save(session)));
  }

  signInCustomer(email: string, password: string): Observable<Session> {
    return this.http
      .post<Session>(`${API_SITE}/api/account/sign-in`, { email, password })
      .pipe(tap((session) => this.save(session)));
  }

  loadProfile(): Observable<StaffSessionProfile> {
    return this.http.get<StaffSessionProfile>(`${API_STAFF}/api/session`).pipe(
      tap((profile) => {
        const current = this.session();
        if (current && current.role !== profile.role) {
          this.save({ ...current, role: profile.role });
        }
        this.profile.set(profile);
      }),
    );
  }

  signOut(): Observable<void> {
    const url = this.role() === 'Cliente' ? `${API_SITE}/api/account/sign-out` : `${API_STAFF}/api/session/sign-out`;
    const request = this.token()
      ? this.http.post(url, {}).pipe(catchError(() => of(null)))
      : of(null);
    return request.pipe(
      tap(() => this.forget()),
      map(() => undefined),
    );
  }

  forget(): void {
    this.session.set(null);
    this.profile.set(null);
    sessionStorage.removeItem(KEY);
  }

  private save(session: Session): void {
    sessionStorage.setItem(KEY, JSON.stringify(session));
    this.session.set(session);
    this.profile.set(null);
  }
}

function read(): Session | null {
  const raw = sessionStorage.getItem(KEY);
  if (!raw) {
    return null;
  }
  try {
    const data = JSON.parse(raw) as Session;
    if (!data.token || !data.email) {
      return null;
    }
    return data;
  } catch {
    return null;
  }
}
