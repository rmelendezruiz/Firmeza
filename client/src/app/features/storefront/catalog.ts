import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { SessionService } from '../../core/session.service';
import { formatMoney } from '../../shared/format';
import { PublishedCar, priceLines, CatalogPart, StorefrontApi } from './storefront-api';
import { Compra } from './purchase';

@Component({
  selector: 'app-catalogo',
  imports: [FormsModule, RouterLink, Compra],
  templateUrl: './catalog.html',
  styleUrl: './storefront.css',
})
export class Catalog implements OnInit {
  private readonly api = inject(StorefrontApi);
  private readonly session = inject(SessionService);
  private readonly router = inject(Router);

  protected vista: 'cars' | 'repuestos' = 'cars';
  protected search = '';
  protected readonly email = this.session.email;
  protected readonly cars = signal<PublishedCar[]>([]);
  protected readonly parts = signal<CatalogPart[]>([]);
  protected readonly listo = signal(false);
  protected readonly error = signal('');
  protected readonly notice = signal('');
  protected readonly enCompra = signal<PublishedCar | null>(null);
  protected readonly moneda = formatMoney;
  protected readonly precios = priceLines;

  ngOnInit(): void {
    this.cargarCarros();
  }

  protected ver(vista: 'cars' | 'repuestos'): void {
    this.vista = vista;
    this.error.set('');
    this.listo.set(false);
    if (vista === 'cars') {
      this.cargarCarros();
      return;
    }
    this.cargarRepuestos();
  }

  protected buscar(): void {
    this.cargarRepuestos();
  }

  protected open(car: PublishedCar): void {
    this.notice.set('');
    this.enCompra.set(car);
  }

  protected items(notice: string): void {
    this.enCompra.set(null);
    this.notice.set(notice);
    this.cargarCarros();
  }

  protected foto(car: PublishedCar): string {
    return car.photoUrl || '/set/unidad.jpg';
  }

  protected signOut(): void {
    this.session.signOut().subscribe(() => {
      void this.router.navigateByUrl('/');
    });
  }

  private cargarCarros(): void {
    this.api.cars().subscribe({
      next: (cars) => {
        this.cars.set(cars);
        this.listo.set(true);
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  private cargarRepuestos(): void {
    this.listo.set(false);
    this.api.parts(this.search).subscribe({
      next: (parts) => {
        this.parts.set(parts);
        this.listo.set(true);
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}
