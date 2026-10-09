import { AfterViewInit, Component, ElementRef, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { PublicBranch, StorefrontApi } from './storefront-api';
import { StorefrontFrame } from './storefront-frame';

@Component({
  selector: 'app-landing',
  imports: [RouterLink, StorefrontFrame],
  templateUrl: './landing.html',
  styleUrl: './storefront.css',
})
export class Landing implements OnInit, AfterViewInit, OnDestroy {
  private readonly api = inject(StorefrontApi);
  private readonly raiz = inject(ElementRef<HTMLElement>);
  private observer: IntersectionObserver | null = null;
  private fondos: IntersectionObserver | null = null;

  protected readonly branches = signal<PublicBranch[]>([]);
  protected readonly error = signal('');
  protected readonly tono = signal<'oscuro' | 'claro'>('oscuro');

  ngOnInit(): void {
    document.documentElement.classList.add('pagina-firmeza');
    this.api.branches().subscribe({
      next: (branches) => this.branches.set(branches),
      error: (error: unknown) => this.error.set(errorMessage(error)),
    });
  }

  ngAfterViewInit(): void {
    const raiz = this.raiz.nativeElement;
    this.observer = new IntersectionObserver(
      (entradas) => {
        for (const entrada of entradas) {
          if (entrada.isIntersecting) {
            entrada.target.classList.add('entra');
          }
        }
      },
      { threshold: 0.18 },
    );
    for (const nodo of raiz.querySelectorAll('.mueve')) {
      this.observer.observe(nodo);
    }

    this.fondos = new IntersectionObserver(
      (entradas) => {
        const visible = entradas
          .filter((entrada) => entrada.isIntersecting)
          .sort((a, b) => b.intersectionRatio - a.intersectionRatio)[0];
        const fondo = visible?.target.getAttribute('data-fondo');
        if (fondo === 'claro' || fondo === 'oscuro') {
          this.tono.set(fondo);
        }
      },
      { rootMargin: '-18% 0px -55% 0px', threshold: [0.2, 0.5, 0.8] },
    );
    for (const nodo of raiz.querySelectorAll('[data-fondo]')) {
      this.fondos.observe(nodo);
    }
  }

  ngOnDestroy(): void {
    document.documentElement.classList.remove('pagina-firmeza');
    this.observer?.disconnect();
    this.fondos?.disconnect();
  }

  protected foto(branch: PublicBranch): string {
    return branch.photos[0]?.url || `/set/counter-${branch.code.toLowerCase()}.jpg`;
  }
}
