import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { errorMessage } from '../../core/error-message';
import { formatMoney } from '../../shared/format';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Headquarters as HeadquartersDto, HeadquartersApi, PaymentMix } from './headquarters-api';

const METODOS: Record<string, string> = {
  efectivo: 'Efectivo',
  transferencia: 'Transferencia',
  tarjeta: 'Tarjeta',
  credito: 'Crédito',
};

@Component({
  selector: 'app-headquarters',
  imports: [FormsModule, TerminalButton],
  templateUrl: './headquarters.html',
  styleUrl: './headquarters.css',
})
export class Headquarters implements OnInit {
  private readonly api = inject(HeadquartersApi);

  protected readonly datos = signal<HeadquartersDto | null>(null);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly error = signal('');
  protected readonly moneda = formatMoney;
  protected period = periodoActual();

  protected readonly metodoTop = computed(() => {
    const pagos = this.datos()?.payments ?? [];
    const porMetodo = new Map<string, number>();
    for (const pago of pagos) {
      porMetodo.set(pago.method, (porMetodo.get(pago.method) ?? 0) + pago.sales);
    }
    const total = pagos.reduce((suma, pago) => suma + pago.sales, 0);
    const [metodo, sales] = [...porMetodo.entries()].sort((a, b) => b[1] - a[1])[0] ?? ['', 0];
    return { metodo: METODOS[metodo] ?? 'Sin ventas', sales, parte: porcentaje(sales, total) };
  });

  protected readonly canales = computed(() => {
    const pagos = this.datos()?.payments ?? [];
    const web = pagos.filter((pago) => pago.channel === 'pagina').reduce((suma, pago) => suma + pago.sales, 0);
    const counter = pagos.filter((pago) => pago.channel !== 'pagina').reduce((suma, pago) => suma + pago.sales, 0);
    return { web, counter, partePagina: porcentaje(web, web + counter), parteCounter: porcentaje(counter, web + counter) };
  });

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    this.api.get(this.period).subscribe({
      next: (datos) => {
        this.datos.set(datos);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected metodo(pago: PaymentMix): string {
    return METODOS[pago.method] ?? pago.method;
  }

  protected channel(pago: PaymentMix): string {
    return pago.channel === 'pagina' ? 'Página' : 'Counter';
  }

  protected parte(pago: PaymentMix): number {
    const total = (this.datos()?.payments ?? []).reduce((suma, row) => suma + row.sales, 0);
    return porcentaje(pago.sales, total);
  }
}

function porcentaje(parte: number, total: number): number {
  return total === 0 ? 0 : Math.round((parte / total) * 100);
}

function periodoActual(): string {
  const hoy = new Date();
  return `${hoy.getFullYear()}-${String(hoy.getMonth() + 1).padStart(2, '0')}`;
}
