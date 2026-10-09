import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { roleName } from '../../core/roles';
import { formatMoney } from '../../shared/format';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Performance as PerformanceDto, ManagementApi, Mensaje } from './management-api';

@Component({
  selector: 'app-performance',
  imports: [FormsModule, TerminalButton],
  templateUrl: './performance.html',
})
export class Performance implements OnInit {
  private readonly api = inject(ManagementApi);

  protected readonly datos = signal<PerformanceDto | null>(null);
  protected readonly messages = signal<Mensaje[]>([]);
  protected readonly error = signal('');
  protected readonly moneda = formatMoney;
  protected readonly roleName = roleName;
  protected period = periodoActual();
  protected con = '';
  protected text = '';

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.error.set('');
    this.api.performance(this.period).subscribe({
      next: (datos) => {
        this.datos.set(datos);
        if (!this.con && datos.contacts.length > 0) {
          this.con = datos.contacts[0].email;
          this.read();
        }
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected read(): void {
    if (!this.con) {
      return;
    }
    this.api.messages(this.con).subscribe({
      next: (items) => this.messages.set(items),
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  protected send(): void {
    this.api.send(this.con, this.text).subscribe({
      next: () => {
        this.text = '';
        this.read();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}

function periodoActual(): string {
  const hoy = new Date();
  return `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}`;
}
