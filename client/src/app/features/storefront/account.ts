import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { SessionService } from '../../core/session.service';
import { StorefrontApi } from './storefront-api';

@Component({
  selector: 'app-account',
  imports: [FormsModule, RouterLink],
  templateUrl: './account.html',
  styleUrl: './storefront.css',
})
export class Account {
  private readonly api = inject(StorefrontApi);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected modo: 'entrar' | 'abrir' = 'entrar';
  protected name = '';
  protected email = '';
  protected document = '';
  protected phone = '';
  protected password = '';
  protected readonly sending = signal(false);
  protected readonly notice = signal('');
  protected readonly error = signal('');

  protected mostrarAbrir(): void {
    this.modo = 'abrir';
    this.notice.set('');
    this.error.set('');
  }

  protected mostrarEntrar(): void {
    this.modo = 'entrar';
    this.notice.set('');
    this.error.set('');
  }

  protected signIn(): void {
    this.error.set('');
    this.notice.set('');
    this.sending.set(true);
    this.session.signInCustomer(this.email, this.password).subscribe({
      next: () => {
        this.sending.set(false);
        void this.router.navigateByUrl('/catalogo');
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected open(): void {
    this.error.set('');
    this.notice.set('');
    this.sending.set(true);
    this.api
      .open({
        name: this.name,
        email: this.email,
        document: this.document,
        phone: this.phone,
        password: this.password,
      })
      .subscribe({
        next: () => {
          this.sending.set(false);
          this.modo = 'entrar';
          this.notice.set('Cuenta abierta. Entra con ese correo.');
          this.password = '';
        },
        error: (error: unknown) => {
          this.sending.set(false);
          this.error.set(errorMessage(error));
        },
      });
  }
}
