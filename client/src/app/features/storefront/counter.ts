import { Component, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { formatMoney } from '../../shared/format';
import { PublishedCar, priceLines, PublicBranch, StorefrontApi } from './storefront-api';
import { StorefrontFrame } from './storefront-frame';

@Component({
  selector: 'app-counter',
  imports: [RouterLink, StorefrontFrame],
  templateUrl: './counter.html',
  styleUrl: './storefront.css',
})
export class Counter implements OnInit, OnDestroy {
  private readonly api = inject(StorefrontApi);
  private readonly ruta = inject(ActivatedRoute);
  private readonly sanitizer = inject(DomSanitizer);

  protected readonly branch = signal<PublicBranch | null>(null);
  protected readonly cars = signal<PublishedCar[]>([]);
  protected readonly error = signal('');
  protected readonly moneda = formatMoney;
  protected readonly precios = priceLines;

  ngOnInit(): void {
    document.documentElement.classList.add('pagina-firmeza');
    this.ruta.paramMap.subscribe((params) => {
      const code = params.get('codigo') ?? '';
      this.load(code);
    });
  }

  ngOnDestroy(): void {
    document.documentElement.classList.remove('pagina-firmeza');
  }

  protected mapa(branch: PublicBranch): string {
    return `https://www.openstreetmap.org/?mlat=${branch.latitude}&mlon=${branch.longitude}#map=16/${branch.latitude}/${branch.longitude}`;
  }

  protected embebido(branch: PublicBranch): SafeResourceUrl {
    const oeste = branch.longitude - 0.012;
    const sur = branch.latitude - 0.008;
    const este = branch.longitude + 0.012;
    const norte = branch.latitude + 0.008;
    return this.sanitizer.bypassSecurityTrustResourceUrl(
      `https://www.openstreetmap.org/export/embed.html?bbox=${oeste}%2C${sur}%2C${este}%2C${norte}&layer=mapnik&marker=${branch.latitude}%2C${branch.longitude}`,
    );
  }

  protected foto(car: PublishedCar): string {
    return car.photoUrl || '/set/unidad.jpg';
  }

  private load(code: string): void {
    this.error.set('');
    this.branch.set(null);
    this.api.branch(code).subscribe({
      next: (branch) => {
        this.branch.set(branch);
        this.api.showcase({ city: branch.city }).subscribe({
          next: (cars) => this.cars.set(cars),
          error: (error: unknown) => this.error.set(errorMessage(error)),
        });
      },
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }
}
