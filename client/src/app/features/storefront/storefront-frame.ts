import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { staffHome } from '../../core/roles';
import { SessionService } from '../../core/session.service';
import { PublicBranch, StorefrontApi } from './storefront-api';

@Component({
  selector: 'app-storefront-frame',
  imports: [RouterLink],
  templateUrl: './storefront-frame.html',
  styleUrl: './storefront.css',
})
export class StorefrontFrame implements OnInit {
  private readonly api = inject(StorefrontApi);
  private readonly router = inject(Router);
  private readonly session = inject(SessionService);

  readonly modo = input<'casa' | 'tienda'>('casa');
  readonly tono = input<'oscuro' | 'claro'>('oscuro');

  protected readonly branches = signal<PublicBranch[]>([]);
  protected readonly esCliente = computed(() => this.session.role() === 'Cliente');
  protected readonly esEquipo = computed(() => this.session.isActive() && !this.esCliente());
  protected readonly casa = computed(() => staffHome(this.session.role()));
  protected readonly marcaVaA = computed(() => (this.modo() === 'tienda' ? '/usados' : '/'));

  ngOnInit(): void {
    this.api.branches().subscribe({ next: (branches) => this.branches.set(branches) });
  }

  protected irMarca(evento: MouseEvent): void {
    if (this.modo() === 'casa' && this.router.url.split('?')[0] === '/') {
      evento.preventDefault();
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  protected mapa(branch: PublicBranch): string {
    return `https://www.openstreetmap.org/?mlat=${branch.latitude}&mlon=${branch.longitude}#map=16/${branch.latitude}/${branch.longitude}`;
  }
}
