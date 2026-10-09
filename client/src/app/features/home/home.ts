import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { errorMessage } from '../../core/error-message';
import { formatMoney } from '../../shared/format';
import { publisher, seller } from '../../core/roles';
import { SessionService } from '../../core/session.service';
import { HomeApi, Mesa } from './home-api';

@Component({
  selector: 'app-home',
  imports: [RouterLink],
  templateUrl: './home.html',
})
export class Home implements OnInit {
  private readonly api = inject(HomeApi);
  private readonly session = inject(SessionService);

  protected readonly status = signal<'cargando' | 'listo' | 'error'>('cargando');
  protected readonly error = signal('');
  protected readonly mesa = signal<Mesa>({
    physicalStore: 0,
    web: 0,
    published: 0,
    available: 0,
    recent: [],
  });
  protected readonly canSell = computed(() => seller(this.session.role()));
  protected readonly puedePublicar = computed(() => publisher(this.session.role()));
  protected readonly moneda = formatMoney;

  ngOnInit(): void {
    this.api.get().subscribe({
      next: (mesa) => {
        this.mesa.set(mesa);
        this.status.set('listo');
      },
      error: (error: unknown) => {
        this.error.set(errorMessage(error));
        this.status.set('error');
      },
    });
  }

  protected channel(valor: string): string {
    return valor === 'pagina' ? 'página' : 'punto físico';
  }
}
