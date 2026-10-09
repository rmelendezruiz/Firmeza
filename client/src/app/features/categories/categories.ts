import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Category, CategoriesApi } from './categories-api';

@Component({
  selector: 'app-categories',
  imports: [FormsModule, TerminalButton],
  templateUrl: './categories.html',
})
export class Categories implements OnInit {
  private readonly api = inject(CategoriesApi);

  protected readonly categories = signal<Category[]>([]);
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
        this.categories.set(items);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected edit(category: Category): void {
    this.editingId.set(category.id);
    this.name = category.name;
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
        this.notice.set(id ? 'categoría actualizada.' : 'categoría creada.');
        this.cancel();
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected remove(category: Category): void {
    this.notice.set('');
    this.error.set('');
    this.api.remove(category.id).subscribe({
      next: () => {
        this.notice.set('categoría eliminada.');
        if (this.editingId() === category.id) {
          this.cancel();
        }
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}
