import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { formatMoney } from '../../shared/format';
import { PublishedCar, ShowcaseFilter, priceLines, StorefrontApi } from './storefront-api';
import { StorefrontFrame } from './storefront-frame';

@Component({
  selector: 'app-usados',
  imports: [FormsModule, RouterLink, StorefrontFrame],
  templateUrl: './usados.html',
  styleUrl: './storefront.css',
})
export class Usados implements OnInit, OnDestroy {
  private readonly api = inject(StorefrontApi);
  private readonly ruta = inject(ActivatedRoute);

  protected readonly cars = signal<PublishedCar[]>([]);
  protected readonly error = signal('');
  protected readonly listo = signal(false);
  protected readonly precios = priceLines;
  protected readonly moneda = formatMoney;
  protected brand = '';
  protected line = '';
  protected city = '';
  protected yearFrom = '';
  protected yearTo = '';
  protected priceFrom = '';
  protected priceTo = '';

  ngOnInit(): void {
    document.documentElement.classList.add('pagina-firmeza');
    const city = this.ruta.snapshot.queryParamMap.get('ciudad');
    if (city) {
      this.city = city;
    }
    this.buscar();
  }

  ngOnDestroy(): void {
    document.documentElement.classList.remove('pagina-firmeza');
  }

  protected buscar(): void {
    this.listo.set(false);
    this.error.set('');
    const filter: ShowcaseFilter = {
      brand: this.brand,
      line: this.line,
      city: this.city,
      yearFrom: this.yearFrom,
      yearTo: this.yearTo,
      priceFrom: this.priceFrom,
      priceTo: this.priceTo,
    };
    this.api.showcase(filter).subscribe({
      next: (cars) => {
        this.cars.set(cars);
        this.listo.set(true);
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.listo.set(true);
      },
    });
  }

  protected limpiar(): void {
    this.brand = '';
    this.line = '';
    this.city = '';
    this.yearFrom = '';
    this.yearTo = '';
    this.priceFrom = '';
    this.priceTo = '';
    this.buscar();
  }

  protected foto(car: PublishedCar): string {
    return car.photoUrl || '/set/unidad.jpg';
  }

  protected price(car: PublishedCar): number {
    return car.pagePrice ?? car.counterPrice ?? 0;
  }
}
