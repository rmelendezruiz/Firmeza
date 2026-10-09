import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { errorMessage } from '../../core/error-message';
import { publisher, seesHeadquarters } from '../../core/roles';
import { SessionService } from '../../core/session.service';
import { formatMoney } from '../../shared/format';
import { TerminalButton } from '../../shared/ui/terminal-button';
import { Car, CarsApi, SaveCar, Branch } from './cars-api';

@Component({
  selector: 'app-cars',
  imports: [FormsModule, TerminalButton],
  templateUrl: './cars.html',
})
export class Cars implements OnInit {
  private readonly api = inject(CarsApi);
  private readonly session = inject(SessionService);

  protected readonly cars = signal<Car[]>([]);
  protected readonly branches = signal<Branch[]>([]);
  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly notice = signal('');
  protected readonly error = signal('');
  protected readonly sending = signal(false);
  protected readonly editingId = signal<string | null>(null);
  protected readonly moneda = formatMoney;
  protected readonly puedePublicar = computed(() => publisher(this.session.role()));
  protected readonly eligeCounter = computed(() => seesHeadquarters(this.session.role()));
  protected readonly miCounter = this.session.branch;

  protected brand = '';
  protected line = '';
  protected year = '';
  protected type = 'automovil';
  protected condition = 'usado';
  protected mileage = '';
  protected availability = 'ambos';
  protected pagePrice = '';
  protected counterPrice = '';
  protected photoUrl = '';
  protected branchId = '';

  ngOnInit(): void {
    this.load();
  }

  protected estadoCarro(car: Car): string {
    if (car.sold) {
      return 'Vendido';
    }
    if (car.availability === 'pagina') {
      return 'Solo página, en bodega';
    }
    if (car.availability === 'counter') {
      return 'Solo counter';
    }
    return 'Página y counter';
  }

  protected precios(car: Car): string {
    const web = car.pagePrice === null ? '' : `Página ${formatMoney(car.pagePrice)}`;
    const counter = car.counterPrice === null ? '' : `Counter ${formatMoney(car.counterPrice)}`;
    return [web, counter].filter(Boolean).join(' · ');
  }

  protected load(): void {
    this.status.set('cargando');
    this.error.set('');
    forkJoin({ cars: this.api.list(), branches: this.api.branches() }).subscribe({
      next: ({ cars, branches }) => {
        this.cars.set(cars);
        this.branches.set(branches);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected edit(car: Car): void {
    this.editingId.set(car.id);
    this.brand = car.brand;
    this.line = car.line;
    this.year = String(car.year);
    this.type = car.type;
    this.condition = car.condition;
    this.mileage = car.mileage === null ? '' : String(car.mileage);
    this.availability = car.availability;
    this.pagePrice = car.pagePrice === null ? '' : String(car.pagePrice);
    this.counterPrice = car.counterPrice === null ? '' : String(car.counterPrice);
    this.photoUrl = car.photoUrl;
    this.branchId = car.branchId;
    this.notice.set('');
    this.error.set('');
  }

  protected cancel(): void {
    this.editingId.set(null);
    this.brand = '';
    this.line = '';
    this.year = '';
    this.type = 'automovil';
    this.condition = 'usado';
    this.mileage = '';
    this.availability = 'ambos';
    this.pagePrice = '';
    this.counterPrice = '';
    this.photoUrl = '';
    this.branchId = '';
  }

  protected save(): void {
    this.notice.set('');
    this.error.set('');
    this.sending.set(true);
    const id = this.editingId();
    const request = id ? this.api.update(id, this.command()) : this.api.create(this.command());
    request.subscribe({
      next: (car) => {
        this.sending.set(false);
        this.notice.set(`${car.brand} ${car.line} ${id ? 'actualizado' : 'publicado'} en ${car.branch}.`);
        this.cancel();
        this.load();
      },
      error: (error: unknown) => {
        this.sending.set(false);
        this.error.set(errorMessage(error));
      },
    });
  }

  protected remove(car: Car): void {
    this.notice.set('');
    this.error.set('');
    this.api.remove(car.id).subscribe({
      next: () => {
        this.notice.set(`${car.brand} ${car.line} eliminado.`);
        if (this.editingId() === car.id) {
          this.cancel();
        }
        this.load();
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  private command(): SaveCar {
    const web = this.availability !== 'counter';
    const counter = this.availability !== 'pagina';
    return {
      brand: this.brand,
      line: this.line,
      year: entero(this.year),
      type: this.type,
      condition: this.condition,
      mileage: this.condition === 'nuevo' ? null : entero(this.mileage),
      availability: this.availability,
      pagePrice: web ? decimal(this.pagePrice) : null,
      counterPrice: counter ? decimal(this.counterPrice) : null,
      photoUrl: this.photoUrl,
      branchId: this.branchId || null,
    };
  }
}

function entero(valor: string): number {
  const parseado = Number(valor.trim());
  return Number.isFinite(parseado) ? parseado : 0;
}

function decimal(valor: string): number {
  const parseado = Number(valor.trim().replace(',', '.'));
  return Number.isFinite(parseado) ? parseado : 0;
}
