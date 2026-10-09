import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { errorMessage } from '../../core/error-message';
import { handlesInspection, seller } from '../../core/roles';
import { SessionService } from '../../core/session.service';
import { formatDate, formatMoney } from '../../shared/format';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Car, CarsApi } from '../cars/cars-api';
import { Customer, CustomersApi } from '../customers/customers-api';
import { Product, ProductsApi } from '../products/products-api';
import { Sale, SalesApi } from './sales-api';

interface LineaFormulario {
  productId: string;
  quantity: string;
}

const METODOS = [
  { valor: 'efectivo', text: 'Efectivo' },
  { valor: 'transferencia', text: 'Transferencia' },
  { valor: 'tarjeta', text: 'Tarjeta' },
  { valor: 'credito', text: 'Crédito' },
];

@Component({
  selector: 'app-sales',
  imports: [FormsModule, TerminalButton],
  templateUrl: './sales.html',
})
export class Sales implements OnInit {
  private readonly ventasApi = inject(SalesApi);
  private readonly clientesApi = inject(CustomersApi);
  private readonly productosApi = inject(ProductsApi);
  private readonly carrosApi = inject(CarsApi);
  private readonly session = inject(SessionService);

  protected readonly sales = signal<Sale[]>([]);
  protected readonly customers = signal<Customer[]>([]);
  protected readonly products = signal<Product[]>([]);
  protected readonly cars = signal<Car[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly moneda = formatMoney;
  protected readonly date = formatDate;
  protected readonly canSell = computed(() => seller(this.session.role()));
  protected readonly handlesInspection = computed(() => handlesInspection(this.session.role()));
  protected readonly miCounter = this.session.branch;

  protected customerId = '';
  protected channel = 'punto';
  protected paymentMethod = 'efectivo';
  protected modo = 'car';
  protected carId = '';
  protected lines: LineaFormulario[] = [{ productId: '', quantity: '1' }];

  protected available(): Car[] {
    return this.cars().filter((car) => !car.sold);
  }

  protected metodos(): typeof METODOS {
    return this.channel === 'pagina' ? METODOS.filter((metodo) => metodo.valor !== 'efectivo') : METODOS;
  }

  protected cambiarCanal(channel: string): void {
    this.channel = channel;
    if (channel === 'pagina' && this.paymentMethod === 'efectivo') {
      this.paymentMethod = 'transferencia';
    }
  }

  protected canalYPago(sale: Sale): string {
    const channel = sale.channel === 'pagina' ? 'Página' : 'Counter';
    const metodo = METODOS.find((opcion) => opcion.valor === sale.paymentMethod)?.text ?? sale.paymentMethod;
    return `${channel} · ${metodo}`;
  }

  protected saleStatus(sale: Sale): string {
    if (sale.status === 'preliminar') {
      return sale.inspectionDeadline ? `En peritaje hasta ${formatDate(sale.inspectionDeadline)}` : 'En peritaje';
    }
    return sale.status === 'anulada' ? 'Anulada' : 'Cerrada';
  }

  protected compraDelCliente(sale: Sale): boolean {
    return sale.channel === 'pagina' && sale.sellerEmail === '';
  }

  ngOnInit(): void {
    this.load();
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    forkJoin({
      sales: this.ventasApi.list(),
      customers: this.clientesApi.list(),
      products: this.productosApi.list('', ''),
      cars: this.carrosApi.list(),
    }).subscribe({
      next: (datos) => {
        this.sales.set(datos.sales);
        this.customers.set(datos.customers);
        this.products.set(datos.products);
        this.cars.set(datos.cars);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected agregarLinea(): void {
    this.lines = [...this.lines, { productId: '', quantity: '1' }];
  }

  protected quitar(indice: number): void {
    this.lines = this.lines.filter((_, i) => i !== indice);
  }

  protected register(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const esCarro = this.modo === 'car';
    this.ventasApi
      .register({
        customerId: this.customerId,
        channel: this.channel,
        paymentMethod: this.paymentMethod,
        carId: esCarro && this.carId ? this.carId : null,
        lines: esCarro
          ? []
          : this.lines.map((line) => ({
              productId: line.productId,
              quantity: number(line.quantity),
            })),
      })
      .subscribe({
        next: (sale) => {
          this.sending.set(false);
          this.notice.set(
            sale.status === 'preliminar'
              ? `Venta ${sale.number} registrada en peritaje hasta ${formatDate(sale.inspectionDeadline ?? sale.date)}.`
              : `Venta ${sale.number} registrada por ${formatMoney(sale.total)}.`,
          );
          this.customerId = '';
          this.carId = '';
          this.lines = [{ productId: '', quantity: '1' }];
          this.load();
        },
        error: (error: unknown) => {
          this.sending.set(false);
          this.error.set(errorMessage(error));
        },
      });
  }

  protected closeInspection(sale: Sale): void {
    this.ejecutar(this.ventasApi.closeInspection(sale.id), `Peritaje de ${sale.number} aprobado. La venta quedó cerrada.`);
  }

  protected cancel(sale: Sale): void {
    this.ejecutar(this.ventasApi.cancel(sale.id), `${sale.number} anulada. El carro volvió a la vitrina.`);
  }

  protected remove(sale: Sale): void {
    this.ejecutar(this.ventasApi.remove(sale.id), `Venta ${sale.number} eliminada.`);
  }

  private ejecutar(request: ReturnType<SalesApi['cancel']>, message: string): void {
    this.notice.set('');
    this.error.set('');
    request.subscribe({
      next: () => {
        this.notice.set(message);
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}

function number(valor: string): number {
  const parseado = Number(valor.trim());
  return Number.isFinite(parseado) ? parseado : 0;
}
