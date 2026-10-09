import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Customer, CustomersApi } from './customers-api';

@Component({
  selector: 'app-customers',
  imports: [FormsModule, TerminalButton],
  templateUrl: './customers.html',
})
export class Customers implements OnInit {
  private readonly api = inject(CustomersApi);

  protected readonly customers = signal<Customer[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly editingId = signal<string | null>(null);

  protected name = '';
  protected email = '';
  protected document = '';
  protected phone = '';

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    this.api.list().subscribe({
      next: (items) => {
        this.customers.set(items);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected edit(customer: Customer): void {
    this.editingId.set(customer.id);
    this.name = customer.name;
    this.email = customer.email;
    this.document = customer.document;
    this.phone = customer.phone;
    this.notice.set('');
    this.error.set('');
  }

  protected cancel(): void {
    this.editingId.set(null);
    this.name = '';
    this.email = '';
    this.document = '';
    this.phone = '';
  }

  protected save(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const command = {
      name: this.name,
      email: this.email,
      document: this.document,
      phone: this.phone,
    };
    const id = this.editingId();
    const request = id ? this.api.update(id, command) : this.api.create(command);
    request.subscribe({
      next: () => {
        this.sending.set(false);
        this.notice.set(id ? 'customer actualizado.' : 'customer creado.');
        this.cancel();
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected remove(customer: Customer): void {
    this.notice.set('');
    this.error.set('');
    this.api.remove(customer.id).subscribe({
      next: () => {
        this.notice.set('customer eliminado.');
        if (this.editingId() === customer.id) {
          this.cancel();
        }
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}
