import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Brand, BrandsApi } from './brands-api';

@Component({
  selector: 'app-brands',
  imports: [FormsModule, TerminalButton],
  templateUrl: './brands.html',
})
export class Brands implements OnInit {
  private readonly api = inject(BrandsApi);

  protected readonly brands = signal<Brand[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly editingId = signal<string | null>(null);

  protected name = '';

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    this.api.list().subscribe({
      next: (items) => {
        this.brands.set(items);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected edit(brand: Brand): void {
    this.editingId.set(brand.id);
    this.name = brand.name;
    this.notice.set('');
    this.error.set('');
  }

  protected cancel(): void {
    this.editingId.set(null);
    this.name = '';
  }

  protected save(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const command = { name: this.name };
    const id = this.editingId();
    const request = id ? this.api.update(id, command) : this.api.create(command);
    request.subscribe({
      next: () => {
        this.sending.set(false);
        this.notice.set(id ? 'brand actualizada.' : 'brand creada.');
        this.cancel();
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected remove(brand: Brand): void {
    this.notice.set('');
    this.error.set('');
    this.api.remove(brand.id).subscribe({
      next: () => {
        this.notice.set('brand eliminada.');
        if (this.editingId() === brand.id) {
          this.cancel();
        }
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}
